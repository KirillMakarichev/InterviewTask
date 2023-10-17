namespace InterviewTask.DataBase.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    
    public List<Friendship> Friendships { get; set; }
    
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();
    private List<Image> _images = new List<Image>();

}