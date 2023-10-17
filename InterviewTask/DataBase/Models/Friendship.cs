namespace InterviewTask.DataBase.Models;

public class Friendship
{
    public int Id { get; set; }
    public int UserRequestingId { get; set; }
    public User UserRequesting { get; set; }
    
    public int UserRespondingId { get; set; }
    public User UserResponding { get; set; }
}