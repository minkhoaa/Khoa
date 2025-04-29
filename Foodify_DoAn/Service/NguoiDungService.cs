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
    }
}