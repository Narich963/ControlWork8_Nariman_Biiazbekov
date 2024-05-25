namespace Forum.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set;} = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; }
}
