using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum Priority
{
    [Display(Name = "Легка")]
    Easy = 0,
    [Display(Name = "Підважлива")]
    Medium = 1,
    [Display(Name = "Важлива")]
    Hard = 2
    
}