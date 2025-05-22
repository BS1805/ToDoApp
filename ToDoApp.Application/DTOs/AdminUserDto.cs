using System.Collections.Generic;

namespace ToDoApp.Application.DTOs
{
    public class AdminUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public int TaskCount { get; set; }

        public List<int> Permissions { get; set; }
    }
}
