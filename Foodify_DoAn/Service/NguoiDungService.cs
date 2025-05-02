using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.OpenApi.MicrosoftExtensions;
using System.Security.Cryptography.Xml;

namespace Foodify_DoAn.Service
{
    public class NguoiDungService : INguoiDungRepository
    {
        private readonly IAccountRepository _account;
        private readonly FoodifyContext _context;
        private readonly UserManager<NguoiDung> _nguoiDung;

        public NguoiDungService(IAccountRepository account, FoodifyContext context)
        {
            _account = account;
            _context = context;
        }

        public async Task<List<NguoiDung>> getAllNguoiDung(string token)
        {
            var user = await _account.AuthenticationAsync(new Model.TokenModel { AccessToken = token });
            if (user == null) return null;

            return await _context.NguoiDungs.ToListAsync();
        }

        public async Task<List<ThongBaoResultDto>> getAllThongBao(string token)
        {
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = token });
            if (user == null) return null;
            var nguoidung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == user.Id);

            var thongBao = await _context.ThongBaos.Where(x => x.MaND == nguoidung.MaND).Select(x => new ThongBaoResultDto
            {
                NgayTao = x.NgayTao,
                DaXem = x.DaXem,
                NoiDung = x.NoiDung
            }).ToListAsync();
            if (thongBao == null) return null;
            return thongBao;
        }

        public async Task<NguoiDungDto> getUserInfor(GetOneUserInfo info)
        {
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = info.token });
            if (user == null) return null;


            var userinfo = await _context.NguoiDungs.Where(x => x.MaND == info.IdUser).Select(x => new NguoiDungDto
            {
                MaTK = x.MaTK,
                TenND = x.TenND,
                GioiTinh = x.GioiTinh,
                NgaySinh = x.NgaySinh,
                TieuSu = x.TieuSu, 
                SDT = x.SDT, 
                Email = x.Email, 
                DiaChi = x.DiaChi,
                LuotTheoDoi = x.LuotTheoDoi,
                AnhDaiDien = x.AnhDaiDien
            }).FirstAsync();
            if (userinfo == null) return null;
            return userinfo;
        }
    }
}