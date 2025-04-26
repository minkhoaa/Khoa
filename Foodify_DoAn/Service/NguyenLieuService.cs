using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;

namespace Foodify_DoAn.Service
{
    public class NguyenLieuService : INguyenLieuRepository
    {

        private readonly FoodifyContext _context;
        private readonly IAccountRepository _account;
        public NguyenLieuService(FoodifyContext context, IAccountRepository account)
        {
            _context = context;
            _account = account;
        }

        public async Task<List<NguyenLieu>> getAllNguyenLieu(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = token});
            if (user == null) return null;

            var nguyenLieus = _context.NguyenLieus.ToList();
            return nguyenLieus;
        }
    }
}
