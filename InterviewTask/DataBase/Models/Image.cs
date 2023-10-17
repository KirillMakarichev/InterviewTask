namespace InterviewTask.DataBase.Models;

public class Image
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long Length { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; }
}