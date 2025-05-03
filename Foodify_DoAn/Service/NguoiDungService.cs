using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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

        public async Task<bool> FollowOneUser(FollowUserDto dto)
        {
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = dto.token });
            if (user == null) return false;

            var followingUser = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == user.Id);
            if (followingUser == null) return false;

            var followedUser = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaND == dto.idTaiKhoan);
            if (followedUser == null) return false;

            var following = new TheoDoi
            {
                Following_ID = followingUser.MaND,
                Followed_ID = followedUser.MaND
            };
            await _context.TheoDois.AddAsync(following);
            followedUser.LuotTheoDoi++;
            _context.Update(followedUser);
            await _context.SaveChangesAsync();
            return true;
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
                MaBaiViet = x.MaBaiViet,
                MaND = x.MaND ,
                MaTB = x.MaTB,
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

            var currentUser = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == user.Id);
            if (currentUser == null) return null;

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
                AnhDaiDien = x.AnhDaiDien,
                isFollowed = _context.TheoDois.Any(a => (a.Following_ID == currentUser.MaND) && (a.Followed_ID == info.IdUser))
            }).FirstAsync();
            if (userinfo == null) return null!;
            return userinfo;
        }

        public async Task<int> SeenOneNotification(SeenOnePost dto)
        {
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = dto.token });
            if (user == null) return -1;
            var noti = await _context.ThongBaos.FirstOrDefaultAsync(x => x.MaTB == dto.idNoti);
            if (noti == null) return -1;

            noti.DaXem = true;
            _context.Update(noti);
            await _context.SaveChangesAsync(); 
            return noti.MaBaiViet;
        }
    }
}