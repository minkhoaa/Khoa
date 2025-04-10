using Foodify_DoAn.Data;
using Foodify_DoAn.Model;

namespace Foodify_DoAn.Repository
{
    public interface IRecipeRepository
    {
        public Task<List<CongThuc>> getAllCongThucs();

        public Task<CongThuc> getByID(int id);

        public Task<CongThuc> addCongThuc(RecipeDto congthuc);

        public Task<CongThuc> updateCongThuc(int id, RecipeDto congthuc);

        public Task<bool> deleteCongThuc(int id);
    }
}
