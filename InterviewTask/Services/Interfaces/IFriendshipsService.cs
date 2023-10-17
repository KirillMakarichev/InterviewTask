using InterviewTask.DataBase.Models;

namespace InterviewTask.Services.Interfaces;

public interface IFriendshipsService
{
    public Task<bool> IsFriendAsync(int idRequesting, int idResponding);
    public Task<bool> AddFriendAsync(int idRequesting, int idResponding);
}