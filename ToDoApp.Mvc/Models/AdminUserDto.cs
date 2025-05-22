namespace ToDoApp.FrontEnd.Models;
public class AdminUserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public IList<string> Roles { get; set; }
    public int TaskCount { get; set; }
}
