using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foodify_DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguyenLieuController : ControllerBase
    {

        private readonly INguyenLieuRepository nguyenLieuRepository;
        public NguyenLieuController(INguyenLieuRepository nguyenLieuRepository)
        {
            this.nguyenLieuRepository = nguyenLieuRepository;
        }

        [HttpPost("getallnguyenlieu")]
        public async Task<IActionResult> getAll([FromBody]string token)
        {
            var result   = await nguyenLieuRepository.getAllNguyenLieu(token);
            if (result == null) return NotFound("Vui lòng đăng nhập");
            return Ok(result); 
        }
        [HttpPost("addnguyenlieu")]
        public async Task<IActionResult> addNguyenLieu(NguyenLieuInputDto nguyenlieu)
        {
            var result = await nguyenLieuRepository.addNguyenLieu(nguyenlieu);
            if (result == null) return BadRequest("Lỗi khi thêm nguyên liệu");
            return Ok(result);
        }
    }
}
