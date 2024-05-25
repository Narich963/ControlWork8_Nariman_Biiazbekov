using Microsoft.AspNetCore.Identity;

namespace Forum.Models;

public class User : IdentityUser<int>
{
    public string Avatar { get; set; }

    public List<Post> Posts { get; set; }
    public User()
    {
        Posts = new List<Post>();
    }
}
