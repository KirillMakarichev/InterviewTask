using InterviewTask.Models;

namespace InterviewTask.Services.Interfaces;

public interface IAuthService
{
    public Task<bool> SignUpAsync(string login, string password);
    public Task<AuthCredentials> Auth(string login, string password);
}