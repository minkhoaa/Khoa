using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace Foodify_DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly IEmailRepository emailSender;
        private readonly FoodifyContext _context; 

        public AccountController(IAccountRepository accountRepository, IEmailRepository emailSender, FoodifyContext context )
        {
            this.accountRepository = accountRepository;
            this.emailSender = emailSender;
            _context = context;
        }
     

        [HttpPost("SignUp/otp")]
        
        public async Task<IActionResult> SignUpWithOtpConfirmed(string email, string otp)
        {
            var result = await accountRepository.SignUpWithOtpAsync(email, otp);
            if (!result)
            {
                return NotFound("Mã otp sai");
            }
            return Ok("Tạo tài khoản thành công");
        }


        [HttpPost("SignUp/SendOtp")]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await accountRepository.SendEmailConfirmationAsync(signUpModel);
            if (!result)
            {
                return NotFound("Không thể gửi mã Otp");
            }

            return Ok("Mã otp được gửi thành công");
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            var result = await accountRepository.SignInAsync(signInModel);

            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(result); 
        }

        [HttpPost("SignIn/Auth")]
        public async Task<IActionResult> Authenticate(TokenModel token)
        {
            var user = await accountRepository.AuthenticationAsync(token);
            return (user == null) ? NotFound("Token không hợp lệ hoặc người dùng không tồn tại") : Ok(new
            {
                user.Email,
                user.UserName
            });
        }

        [HttpPost("Update")]
        [Authorize]
      
        public async Task<IActionResult> UpdateInformationUser([FromBody] UpdateUserInfoModel model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var result = await accountRepository.UpdateInformationUser(userEmail, model);
            if (result == null) return BadRequest("Không tìm thấy người dùng.");
            return Ok();
        }



    }
}
