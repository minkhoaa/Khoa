using Foodify_DoAn.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.EntityFrameworkCore;

namespace Foodify_DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        public readonly FoodifyContext _context;
        public NguoiDungController(FoodifyContext context )
        {
            _context = context;
        }

        [HttpGet]
        public async  Task<IActionResult> getAllNguoiDungs()
        {
            var users = await _context.NguoiDungs.ToListAsync();
            return  Ok(users);
        }
    }
}
