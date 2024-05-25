using Microsoft.AspNetCore.Identity;

namespace Forum.Models;

public class User : IdentityUser<int>
{
    public string Avatar { get; set; }

    public List<Post> Posts { get; set; }
    public List<Comment> Comments { get; set; }
    public User()
    {
        Posts = new List<Post>();
        Comments = new List<Comment>();
    }
}
