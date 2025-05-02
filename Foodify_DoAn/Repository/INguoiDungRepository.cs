using Foodify_DoAn.Data;
using Foodify_DoAn.Model;

namespace Foodify_DoAn.Repository
{
    public interface INguoiDungRepository
    {

        public Task<List<NguoiDung>> getAllNguoiDung(string token);

        public Task<List<ThongBaoResultDto>> getAllThongBao(string token);

        public Task<NguoiDungDto> getUserInfor(GetOneUserInfo info);
    }
}
