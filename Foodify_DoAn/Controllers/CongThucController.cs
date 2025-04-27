using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System.Net.WebSockets;

namespace Foodify_DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongThucController : ControllerBase
    {
        private readonly IRecipeRepository _repository;

        public CongThucController(IRecipeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> getAllCongthucs(string token)
        {
             var congthucs = await _repository.getAllCongThucs(token);
                
            return Ok(congthucs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getCongThucByID(int id)
        {
            var congthuc = await _repository.getByID(id);
            if (congthuc == null) return NotFound("Công thức không tồn tại");
            return Ok(congthuc);
        }

        [HttpPost]
        public async Task<IActionResult> addCongThuc(string token, [FromBody]RecipeDto recipe)
        {
            var congthuc = await _repository.addCongThuc(token,recipe);
            if (congthuc == null) return NotFound("không thể thêm công thức, vui lòng đăng nhập");
            return Ok(congthuc);
    
        }

        [HttpPut]
        public async Task<IActionResult> updateCongThuc(int id, RecipeDto recipe)
        {
            var congthuc = await _repository.updateCongThuc(id, recipe);
            if (congthuc == null) return NotFound("Không thể chỉnh sửa công thức");
            return Ok(congthuc);
        }
        [HttpDelete]
        public async Task<IActionResult> deleteCongThuc(int id)
        {
            var congthuc = await _repository.deleteCongThuc(id);
            if (congthuc == false) return NotFound("Không thể xóa công thức");
            return Ok(congthuc); 
        }
    }
}
