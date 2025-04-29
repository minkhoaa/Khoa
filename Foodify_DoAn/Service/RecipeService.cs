using AutoMapper;
using Castle.Core.Logging;
using DotNetEnv;
using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Npgsql.PostgresTypes;
using Npgsql.Replication;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;

namespace Foodify_DoAn.Service
{
    public class RecipeService : IRecipeRepository
    {
        private readonly FoodifyContext _context;
        private readonly IMapper _dbMapper;
        private readonly IAccountRepository _account;


        public RecipeService(FoodifyContext context, IMapper dbMapper, IAccountRepository accountRepository)
        {
            _context = context;
            _dbMapper = dbMapper;
            _account = accountRepository;
        }

        public async Task<RecipeResultDto> addCongThuc( RecipeDto congthuc)
        {
            if (string.IsNullOrEmpty(congthuc.token)) return null;
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = congthuc.token });
            if (user == null) return null;

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == user.Id);
            if (nguoiDung == null) return null; 


            CongThuc recipe = new CongThuc();
             _dbMapper.Map(congthuc, recipe);
            recipe.MaND = nguoiDung.MaND;
            recipe.NguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaND == nguoiDung.MaND);
            await _context.CongThucs.AddAsync(recipe);
            await _context.SaveChangesAsync();

            List<NguyenLieuDto> nguyenLieuList = new List<NguyenLieuDto>();

            foreach (var nl in congthuc.NguyenLieus)
            {
                var CtCongThuc = new CTCongThuc
                {
                    MaCT = recipe.MaCT,
                    MaNL = nl.MaNL,
                    DinhLuong = nl.DinhLuong,
                    DonViTinh = nl.DonViTinh
                };
                await _context.CTCongThucs.AddAsync(CtCongThuc);

                nguyenLieuList.Add(new NguyenLieuDto
                {
                    MaNL = nl.MaNL,
                    DinhLuong = nl.DinhLuong,
                    DonViTinh = nl.DonViTinh
                });
            }

            await _context.SaveChangesAsync();

            var result = new RecipeResultDto
            {
                MaCT = recipe.MaCT,
                TenCT = recipe.TenCT,
                NguyenLieus = nguyenLieuList
            };

            return result;
        }

        public async Task<bool> deleteCongThuc(int id)
        {
            var congthuc = await _context.CongThucs.FindAsync(id);
            if (congthuc == null) return false;
            _context.Remove(congthuc);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<List<PostResultDto>> getAllCongThucs(RecipeRequestDto recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            int skip = (recipe.PageNumber - 1) * recipe.PageSize;
            int take = recipe.PageSize;

            // Authenticate user once
            var user = string.IsNullOrEmpty(recipe.Token)
                ? null
                : await _account.AuthenticationAsync(new TokenModel { AccessToken = recipe.Token });

            List<int> followingUserIds = new();

            if (user != null)
            {
                followingUserIds = await _context.TheoDois
                    .Where(x => x.Following_ID == user.Id)
                    .Select(a => a.Followed_ID)
                    .ToListAsync();
            }

            // Query recipes along with their authors
            var recipeDtos = await _context.CongThucs
                .Join(_context.NguoiDungs,
                    ct => ct.MaND,
                    nd => nd.MaND,
                    (ct, nd) => new { CongThuc = ct, TacGia = nd })
                .OrderByDescending(x => followingUserIds.Contains(x.CongThuc.MaND))
                .ThenByDescending(x => x.CongThuc.NgayCapNhat)
                .Skip(skip)
                .Take(take)
                .Select(x => new PostResultDto
                {
                    TenCT = x.CongThuc.TenCT,
                    MoTaCT = x.CongThuc.MoTaCT,
                    TongCalories = x.CongThuc.TongCalories,
                    AnhCT = x.CongThuc.AnhCT,
                    LuotXem = x.CongThuc.LuotXem,
                    LuotLuu = x.CongThuc.LuotLuu,
                    LuotThich = x.CongThuc.LuotThich,
                    TacGia = new NguoiDungDto
                    {
                        MaND = x.TacGia.MaND,
                        MaTK = x.TacGia.MaTK,
                        TenND = x.TacGia.TenND,
                        GioiTinh = x.TacGia.GioiTinh,
                        NgaySinh = x.TacGia.NgaySinh,
                        TieuSu = x.TacGia.TieuSu,
                        SDT = x.TacGia.SDT,
                        Email = x.TacGia.Email,
                        DiaChi = x.TacGia.DiaChi,
                        LuotTheoDoi = x.TacGia.LuotTheoDoi,
                        AnhDaiDien = x.TacGia.AnhDaiDien
                    }
                })
                .ToListAsync();

            return recipeDtos;
        }


        public async Task<CongThuc> getByID(int id)
        {
            var recipe = await _context.CongThucs.FindAsync(id);
            if (recipe == null) return null!;
            return recipe;
        }

        public async Task<CongThuc> updateCongThuc(int id, RecipeDto congthuc)
        {
            var recipe = await _context.CongThucs.FindAsync(id);
            if (recipe == null) return null!;
            _dbMapper.Map(congthuc, recipe);
            _context.Update(recipe); 
            await _context.SaveChangesAsync(); 

            return recipe;
        }
        
    }
}
