using Foodify_DoAn.Data;
using Foodify_DoAn.Model;

namespace Foodify_DoAn.Repository
{
    public interface IRecipeRepository
    {
        public Task<List<CongThuc>> getAllCongThucs(string token, int pageNumber = 1, int pageSize = 10);

        public Task<CongThuc> getByID(int id);

        public Task<CongThuc> addCongThuc(string token,RecipeDto congthuc);

        public Task<CongThuc> updateCongThuc(int id, RecipeDto congthuc);

        public Task<bool> deleteCongThuc(int id);

        
    }
}
