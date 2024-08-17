using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModel;

public class TaskViewModel
{
    public long Id { get; set; }
    
    [Display(Name = "Назва")]
    public string Name { get; set; }
    
    [Display(Name = "Стадія виконання")]
    public string IsDone { get; set; }
    
    [Display(Name = "Опис")]
    public string Description { get; set; }
    
    [Display(Name = "Складність")]
    public string Priority { get; set; }
    
    [Display(Name = "Дата створення")]
    public string Created { get; set; }
}