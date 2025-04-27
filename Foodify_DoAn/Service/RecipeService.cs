using AutoMapper;
using Castle.Core.Logging;
using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
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

        public async Task<CongThuc> addCongThuc(string token, RecipeDto congthuc)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = token });
            if (user == null) return null;

            CongThuc recipe = new CongThuc();
            _dbMapper.Map(congthuc, recipe);
            recipe.MaND = user.NguoiDung.MaND;
            _context.CongThucs.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe; 
        }

        public async Task<bool> deleteCongThuc(int id)
        {
            var congthuc = await _context.CongThucs.FindAsync(id);
            if (congthuc == null) return false;
            _context.Remove(congthuc);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<List<CongThuc>> getAllCongThucs(string token, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(token)) return await _context.CongThucs.OrderByDescending(x => x.NgayCapNhat).Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToListAsync();

            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = token });
            if (user == null) return await _context.CongThucs.OrderByDescending(x => x.NgayCapNhat).Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToListAsync();

            var followingUser = await _context.TheoDois.Where(x => x.Following_ID == user.Id).Select(a => a.Followed_ID).ToListAsync();

            var recipes = await _context.CongThucs
            .OrderByDescending(c => followingUser.Contains(c.MaND)) 
            .ThenByDescending(c => c.NgayCapNhat) 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
             .ToListAsync();

            return recipes;
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
