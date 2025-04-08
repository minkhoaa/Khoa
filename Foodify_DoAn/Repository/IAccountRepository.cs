using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace Foodify_DoAn.Repository
{
    public interface IAccountRepository
    {
        public Task<bool> SignUpWithOtpAsync(string email, string otp);
        public Task<string> SignInAsync(SignInModel signInModel);
        public Task<TaiKhoan?> AuthenticationAsync(TokenModel model);
        public Task<bool> SendEmailConfirmationAsync(SignUpModel signUpModel);
        public Task<NguoiDung?> UpdateInformationUser(string email, UpdateUserInfoModel model);

    }
}
