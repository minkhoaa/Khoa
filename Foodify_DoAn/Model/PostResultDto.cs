using Foodify_DoAn.Data;

namespace Foodify_DoAn.Model
{
    public class PostResultDto
    {
        public string TenCT { get; set; } = null!;
        public string MoTaCT { get; set; } = null!;
        public decimal TongCalories { get; set; }
        public string AnhCT { get; set; } = null!;
        public int LuotXem { get; set; }
        public int LuotLuu { get; set; }
        public int LuotThich { get; set; }

        public NguoiDungDto TacGia {get; set; }= null!;

        public List<NguyenLieuOutputDto> NguyenLieus { get; set; }

    }
}
