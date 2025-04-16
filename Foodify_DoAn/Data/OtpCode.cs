using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Foodify_DoAn.Data
{
    [Table("OtpCode")]
    public class OtpCode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; } = default!;
        [Required, MaxLength(6)]
        public string Code { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        public DateTime ExpiredAt { get; set; }
    }
}
