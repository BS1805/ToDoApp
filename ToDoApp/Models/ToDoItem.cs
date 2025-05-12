namespace ToDoApp.Models;

public class ToDoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
