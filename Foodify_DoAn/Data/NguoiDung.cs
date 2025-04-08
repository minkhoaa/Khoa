using System.ComponentModel.DataAnnotations;

namespace Foodify_DoAn.Data
{
    public class NguoiDung
    {
        [Key]
        public int MaND { get; set; }
        public int MaTK { get; set; }
        public string TenND { get; set; }
        public bool? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TieuSu { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public int LuotTheoDoi { get; set; }
        public string AnhDaiDien { get; set; }

        public TaiKhoan TaiKhoan { get; set; }
        public ICollection<DanhGia> DanhGias { get; set; }
        public ICollection<CTDaLuu> CTDaLuus { get; set; }
        public ICollection<CTDaThich> CTDaThichs { get; set; }
        public ICollection<ThongBao> ThongBaos { get; set; }
        public ICollection<TheoDoi> Followers { get; set; }
        public ICollection<TheoDoi> Followeds { get; set; }
    }
}
