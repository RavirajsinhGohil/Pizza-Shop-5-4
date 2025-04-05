using Repository.Models;

namespace Service.Interfaces;

public interface IAuthService
{
    User Login(string email, string password);
    string GenerateJwtToken(string email, int RoleId);
    string GenerateJwtTokenForgot(User user, bool rememberMe);
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    bool  CheckEmailExist(string email);
}
