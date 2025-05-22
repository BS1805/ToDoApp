using System.ComponentModel.DataAnnotations;

namespace ToDoApp.FrontEnd.Models
{
    public class TaskViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Status field is required.")]
        public int StatusId { get; set; }

    }



    public class PagedListViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }
}
