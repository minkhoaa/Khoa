using FluentEmail.Core;
using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Foodify_DoAn.Service
{
    public class AccountManager : IAccountRepository
    {
        private readonly UserManager<TaiKhoan> userManager;
        private readonly SignInManager<TaiKhoan> signInManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<VaiTro> roleManager;
        private readonly IFluentEmail fluentEmail;
        private readonly IMemoryCache memoryCache;
        private readonly FoodifyContext foodifyContext;

        public AccountManager(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager,
            RoleManager<VaiTro> roleManager, IConfiguration configuration, IFluentEmail fluentEmail,
            IMemoryCache memoryCache, FoodifyContext foodifyContext
            )
        {
            this.foodifyContext = foodifyContext;
            this.memoryCache = memoryCache;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.fluentEmail = fluentEmail;
        }
        public async Task<TaiKhoan?> AuthenticationAsync(TokenModel token)
        {
            if (string.IsNullOrEmpty(token.AccessToken)) return null;

            var tokenHanler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!);

            try
            {
                tokenHanler.ValidateToken(token.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidAudience = configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email)) return null;
                var user = await userManager.FindByEmailAsync(email);
                return user;
            }
            catch
            {
                return null;
            }

        }
        public async Task<string> SignInAsync(SignInModel signInModel)
        {
            var user = await userManager.FindByEmailAsync(signInModel.Email);

            var passwordValid = await userManager.CheckPasswordAsync(user, signInModel.Password);
            if (user == null || !passwordValid) return string.Empty;

            var authClaims = new List<Claim>
                { new Claim(ClaimTypes.Email, signInModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, signInModel.Email)

            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));


            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
                );


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public async Task<bool> SendEmailConfirmationAsync(SignUpModel signUpModel)
        {
            var userExist = await userManager.FindByEmailAsync(signUpModel.Email);
            if (userExist != null) return false;

            try
            {
                var otp = new Random().Next(100000, 999999).ToString();
                memoryCache.Set($"OTP_{signUpModel.Email}", new
                {
                    Otp = otp,
                    Password = signUpModel.Password
                }, TimeSpan.FromMinutes(1));
                await fluentEmail.To(signUpModel.Email)
                    .SetFrom("noreply@foodify.com")
                    .Subject("Mã OTP xác thực email")
                    .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 1 phút).</p>", true)
                    .SendAsync();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> SignUpWithOtpAsync(string email, string otp)
        {
            if (!memoryCache.TryGetValue($"OTP_{email}", out dynamic? cacheData)) return false;

            if (cacheData.Otp != otp) return false;
            var user = new TaiKhoan
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, cacheData.Password);
            if (!result.Succeeded) return false;
            if (!await roleManager.RoleExistsAsync(RoleModel.User))
            {
                var role = new VaiTro { Name = RoleModel.User };
                await roleManager.CreateAsync(role);
            }

            await userManager.AddToRoleAsync(user, RoleModel.User);

            var nguoiDung = new NguoiDung
            {
                TenND = "user" + new Random().Next(1, maxValue: 99999),
                MaTK = user.Id,
                Email = user.Email,
                GioiTinh = true,
                NgaySinh = DateTime.UtcNow,
                TieuSu = "no",
                SDT = "no",
                DiaChi = "no",
                AnhDaiDien = "image",
                TaiKhoan = user
            };

            await foodifyContext.NguoiDungs.AddAsync(nguoiDung);
            await foodifyContext.SaveChangesAsync();

            memoryCache.Remove($"OTP_{email}");

            return true;

        }

        public async Task<NguoiDung?> UpdateInformationUser(string email, UpdateUserInfoModel model)
        {
            var taiKhoan = await userManager.FindByEmailAsync(email);
            if (taiKhoan == null) return null;

            var nguoiDung = await foodifyContext.NguoiDungs
                .FirstOrDefaultAsync(x => x.MaTK == taiKhoan.Id);

            if (nguoiDung == null) return null;

            // Cập nhật dữ liệu
            nguoiDung.TenND = model.TenND;
            nguoiDung.NgaySinh = model.NgaySinh;
            nguoiDung.GioiTinh = model.GioiTinh;
            nguoiDung.SDT = model.SDT;
            nguoiDung.DiaChi = model.DiaChi;
            nguoiDung.TieuSu = model.TieuSu;

            await foodifyContext.SaveChangesAsync();

            return nguoiDung;
        }

    }
}
