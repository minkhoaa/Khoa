using AutoMapper;
using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.EntityFrameworkCore;
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

        public RecipeService(FoodifyContext context, IMapper dbMapper)
        {
            _context = context;
            _dbMapper = dbMapper;
        }
        public async Task<CongThuc> addCongThuc(RecipeDto recipe)
        {
            CongThuc congthuc = new CongThuc();
             _dbMapper.Map(recipe, congthuc);
            await _context.AddAsync(congthuc); 
            await _context.SaveChangesAsync();
            return congthuc;

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
