using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extenstions;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModel;
using ToDoList.Service.Interfaces;


namespace ToDoList.Service.Implementations;

public class TaskService : ITaskService
{ 
    private ILogger<TaskService> _logger;
    private readonly IBaseRepository<TaskEntity> _taskRepository;

    public TaskService(ILogger<TaskService> logger, IBaseRepository<TaskEntity> taskRepository)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();
            
            
            _logger.LogInformation($"Запит на створення завдання - {model.Name}");
            var task = await _taskRepository.GetAll().Where(x => x.Created.Date == DateTime.Today).FirstOrDefaultAsync(x => x.Name == model.Name);

            if (task != null)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Завдання з такою назвою вже існує",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            task = new TaskEntity()
            {
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                Created = DateTime.Now
                
            };

            await _taskRepository.Create(task);
            _logger.LogInformation($"Завдання додано {task.Name} {task.Created}");
            
            return new BaseResponse<TaskEntity>()
            {
                Description = "Завдання додано",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.Create]: {e.Message}");
            return new BaseResponse<TaskEntity>()
            {
                Description = $"{e.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x=> !x.IsDone)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Select(x=> new TaskViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsDone = x.IsDone == true ? "Виконана" : "Не виконана",
                Priority = x.Priority.GetDisplayName(),
                Created = x.Created.ToLongDateString(),
            }).Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();
            var count = _taskRepository.GetAll().Count();

            return new DataTableResult()
            {
                Data = tasks,
                Total = count
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.Create]: {e.Message}");
            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id)
    {
        try
        {
            var task = await _taskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Завдання не знайдено",
                    StatusCode = StatusCode.TaskWasNotFound
                };
            }

            task.IsDone = true;

            await _taskRepository.Update(task);

            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.OK,
                Description = "Завдання завершено"
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.EndTask]: {e.Message}");
            return new BaseResponse<bool>()
            {
                Description = $"{e.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskCompletedViewModel>>> GetCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.IsDone)
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskCompletedViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();

            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.EndTask]: {e.Message}");
            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetAll().Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsDone = x.IsDone == true ? "Виконана" : "Не виконана",
                    Priority = x.Priority.GetDisplayName(),
                    Created = x.Created.ToString(CultureInfo.InvariantCulture),
                }).ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.EndTask]: {e.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}