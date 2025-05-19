using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs;

public class TaskViewModel
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "The Title field is required.")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "The Description field is required.")]
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } 
    public string StatusName { get; set; } = string.Empty;
}