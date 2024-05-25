using Forum.Models;

namespace Forum.ViewModels;

public class PaginationViewModel
{
    public List<Comment>? Comments { get; set; }
    public List<Post>? Posts { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
