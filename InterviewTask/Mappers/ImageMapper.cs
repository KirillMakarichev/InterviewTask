using InterviewTask.DataBase.Models;
using InterviewTask.Models;

namespace InterviewTask.Mappers;

public static class ImageMapper
{
    public static ImageResponse ConvertFromDBImage(Image image) => new ImageResponse()
    {
        Id = image.Id,
        Length = image.Length,
        Name = image.Name
    };
}