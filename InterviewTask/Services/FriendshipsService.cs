using InterviewTask.DataBase;
using InterviewTask.DataBase.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewTask.Services;

public class FriendshipsService : IFriendshipsService
{
    private readonly UsersContext _context;

    public FriendshipsService(UsersContext context)
    {
        _context = context;
    }

    public async Task<bool> IsFriendAsync(int idRequesting, int idResponding)
    {
        var isFriend =
            await _context.Friendships.AnyAsync(x => x.UserRequestingId == idRequesting && x.UserRespondingId == idResponding);
        return isFriend;
    }

    public async Task<bool> AddFriendAsync(int idRequesting, int idResponding)
    {
        if(idRequesting == idResponding)
        {
            return false;
        }

        var isFriend = await IsFriendAsync(idRequesting, idResponding);
        if (isFriend)
        {
            return false;
        }

        _context.Friendships.Add(new Friendship()
        {
            UserRequestingId = idRequesting,
            UserRespondingId = idResponding
        });

        await _context.SaveChangesAsync();

        return true;
    }
}