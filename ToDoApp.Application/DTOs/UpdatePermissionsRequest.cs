using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs;
public class UpdatePermissionsRequest
{
    public string UserId { get; set; }
    public UserPermission Permissions { get; set; }
}