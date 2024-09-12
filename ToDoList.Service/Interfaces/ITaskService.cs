using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModel;

namespace ToDoList.Service.Interfaces;

public interface ITaskService
{
    Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model);
    
    Task<DataTableResult> GetTasks(TaskFilter filter);

    Task<IBaseResponse<bool>> EndTask(long id);

    Task<IBaseResponse<IEnumerable<TaskCompletedViewModel>>> GetCompletedTasks();

    Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks();
    
    Task<IBaseResponse<TaskViewModel>> GetDetailedTask(long id);
    
    Task<IBaseResponse<TaskViewModel>> GetByIdAsync(long id);

    Task<IBaseResponse<bool>> UpdateTask(TaskViewModel model);
}