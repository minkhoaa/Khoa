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

        public async Task<List<CongThuc>> getAllCongThucs()
        {
            return await _context.CongThucs.ToListAsync();
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
