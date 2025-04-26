using Foodify_DoAn.Data;

namespace Foodify_DoAn.Repository
{
    public interface INguyenLieuRepository
    {
        Task<List<NguyenLieu>>getAllNguyenLieu(string token);
    };
}
