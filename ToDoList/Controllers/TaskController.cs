using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Helpers;
using ToDoList.Domain.ViewModel;
using ToDoList.Domain.Enum;
using ToDoList.Service.Implementations;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly UserManager<AppUser> _userManager;
    
    public TaskController(ITaskService taskService, UserManager<AppUser> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;

        var respose = await _taskService.Create(model, userId);
        if (respose.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = respose.Description });
        }

        return BadRequest(new { description = respose.Description });
    }

    [HttpPost]
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();

        var pageSize = length != null ? Convert.ToInt32(length) : 0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.PageSize = pageSize;
        filter.Skip = skip;
        
        var response = await _taskService.GetTasks(filter, userId);
        return Json(new { recordsFiltered = response.Total ,recordsTotal = response.Total, response.Data});
    }

    [HttpPost]
    public async Task<IActionResult> EndTask(long id)
    {
        var response = await _taskService.EndTask(id);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }

        return BadRequest(new { description = response.Description });
    }

    public async Task<IActionResult> GetCompletedTasks()
    {
        var result = await _taskService.GetCompletedTasks();
        return Json(new { data = result.Data});
    }

    [HttpPost]
    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await _taskService.CalculateCompletedTasks();

        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            var csvService = new CsvBaseService<TaskViewModel>();
            var uploadFile = csvService.UploadFile(response.Data);
            return File(uploadFile, "text/csv", $"Завдання за сьогодні {DateTime.Today.ToLongDateString()}.csv");
        }

        return BadRequest(new { description = response.Description});
    }

    public async Task<IActionResult> DetailDescription(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        
        var response = await _taskService.GetDetailedTask(id, userId);
        
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> TaskEditPage(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        
        var task = await _taskService.GetByIdAsync(id, userId);
        if (task.StatusCode != Domain.Enum.StatusCode.OK || task.Data == null)
        {
            return View("Error");
        }
        
        return View(task.Data);
    }

    [HttpPost]
    public async Task<IActionResult> TaskEditPage(TaskViewModel model)
    {
        if (!ModelState.IsValid && model.Created != null)
        {
            ModelState.AddModelError("", "Не можливо змінити");
            return View(model);
        }

        await _taskService.UpdateTask(model);
        /*return View(model);*/
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> TaskDeletePage(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        
        var task = await _taskService.GetByIdAsync(id, userId);
        if (task.StatusCode != Domain.Enum.StatusCode.OK || task.Data == null)
        {
            return View("Error");
        }
        
        return View(task.Data);
    }

    [HttpPost]
    public async Task<IActionResult> TaskDeletePage(TaskViewModel model)
    {
        if (!ModelState.IsValid && model.Created != null)
        {
            ModelState.AddModelError("", "Не можливо видалити");
            return View(model);
        }

        await _taskService.DeleteTask(model);
        /*return View(model);*/
        return RedirectToAction("Index");
    }
    
}