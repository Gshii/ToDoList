using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Entity;

public class TaskEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsDone { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime Created { get; set; }
    
    public string AppUserId { get; set; }
    
    [ForeignKey("AppUserId")]
    public AppUser AppUser { get; set; }
}