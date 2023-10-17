using InterviewTask.DataBase.Models;
using InterviewTask.Models;

namespace InterviewTask.Services.Interfaces;

public interface IUsersService
{
    public Task<ReturnStatus<IEnumerable<Image>, Status>> GetAllImagesAsync(string ownerLogin, int requesterId);
    public Task<ReturnStatus<Stream, Status>> GetImageAsync(string imageName, string ownerLogin, int requesterId);
    public Task<Image> SaveImageAsync(Stream image, string fileExtension, int ownerId);
    public Task<User?> GetUserByLoginAsync(string login, bool includeImages = false);
}