namespace ToDoApp.FrontEnd.Models;

public class UpdatePermissionsRequest
{
    public string UserId { get; set; }
    public List<int> Permissions { get; set; }
}