namespace ToDoApp.Domain.Entities;
public class ToDoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;
}
