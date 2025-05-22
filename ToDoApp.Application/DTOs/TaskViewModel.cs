using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs;

public class TaskViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "The Title field is required.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "The Description field is required.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "The Status field is required.")]
    public int StatusId { get; set; }
    // Remove: public string StatusName { get; set; }
}

