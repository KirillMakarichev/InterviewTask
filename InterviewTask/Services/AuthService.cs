using InterviewTask.DataBase;
using InterviewTask.DataBase.Models;
using InterviewTask.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewTask.Services;

public class AuthService : IAuthService
{
    private readonly UsersContext _context;

    public AuthService(UsersContext context)
    {
        _context = context;
    }

    public async Task<bool> SignUpAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (_context.Users.Any(x => x.Login == login))
        {
            return false;
        }

        _context.Users.Add(new User() { Login = login, Password = password });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AuthCredentials> Auth(string login, string password)
    {
        var credentials = new AuthCredentials();
        var authResult = await _context.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
        var userFound = authResult != null;
        if (userFound)
        {
            credentials.Id = authResult.Id;
        }

        credentials.Authenticated = userFound;
        credentials.Login = login;

        return credentials;
    }
}