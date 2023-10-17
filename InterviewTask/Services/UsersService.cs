using InterviewTask.DataBase;
using InterviewTask.DataBase.Models;
using InterviewTask.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InterviewTask.Services;

public class UsersService : IUsersService
{
    private readonly IFriendshipsService _friendshipsService;
    private readonly UsersContext _context;
    private readonly StorageOptions _storageOptions;

    public UsersService(IFriendshipsService friendshipsService, UsersContext context,
        IOptions<StorageOptions> storageOptions)
    {
        _friendshipsService = friendshipsService;
        _context = context;
        _storageOptions = storageOptions.Value;
    }

    public async Task<ReturnStatus<IEnumerable<Image>, Status>> GetAllImagesAsync(string ownerLogin,
        int requesterId)
    {
        var owner = await GetUserByLoginAsync(ownerLogin, true);
        if (owner == null)
        {
            return ReturnStatus<IEnumerable<Image>, Status>.Create(new List<Image>(), Status.OwnerNotFound);
        }

        if (requesterId == owner.Id)
        {
            return ReturnStatus<IEnumerable<Image>, Status>.Create(owner.Images, Status.Ok);
        }

        var isFriend = await _friendshipsService.IsFriendAsync(owner.Id, requesterId);

        if (!isFriend)
        {
            return ReturnStatus<IEnumerable<Image>, Status>.Create(new List<Image>(), Status.NoAccess);
        }

        return ReturnStatus<IEnumerable<Image>, Status>.Create(owner.Images, Status.Ok);
    }

    public async Task<ReturnStatus<Stream, Status>> GetImageAsync(string imageName, string ownerLogin, int requesterId)
    {
        var owner = await GetUserByLoginAsync(ownerLogin, true);

        if (owner == null)
        {
            return ReturnStatus<Stream, Status>.Create(Stream.Null, Status.OwnerNotFound);
        }

        var isFriend = await _friendshipsService.IsFriendAsync(owner.Id, requesterId);
        if (requesterId != owner.Id && !isFriend)
        {
            return ReturnStatus<Stream, Status>.Create(Stream.Null, Status.NoAccess);
        }

        var filePath = Path.Combine(_storageOptions.Path, imageName);

        if (!File.Exists(filePath))
        {
            return ReturnStatus<Stream, Status>.Create(Stream.Null, Status.FileNotFound);
        }

        return ReturnStatus<Stream, Status>.Create(File.OpenRead(filePath), Status.Ok);
    }

    public async Task<Image> SaveImageAsync(Stream imageBytes, string fileExtension, int ownerId)
    {
        var image = new Image()
        {
            Length = imageBytes.Length,
            Name = Guid.NewGuid() + fileExtension,
            OwnerId = ownerId,
        };

        _context.Images.Add(image);

        if (!Directory.Exists(_storageOptions.Path))
        {
            Directory.CreateDirectory(_storageOptions.Path);
        }

        var filePath = Path.Combine(_storageOptions.Path, image.Name);
        await using var fileStream = new FileStream(filePath, FileMode.Create);

        await Task.WhenAll(_context.SaveChangesAsync(), imageBytes.CopyToAsync(fileStream));

        return image;
    }

    public async Task<User?> GetUserByLoginAsync(string login, bool includeImages = false)
    {
        var users = _context.Users.AsQueryable();
        if (includeImages)
        {
            users = users.Include(x => x.Images);
        }

        var user = await users.FirstOrDefaultAsync(x => x.Login == login);

        return user;
    }
}