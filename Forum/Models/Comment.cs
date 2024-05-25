﻿namespace Forum.Models;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }
}
