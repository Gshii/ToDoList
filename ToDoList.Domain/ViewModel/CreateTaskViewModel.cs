using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModel;

public class CreateTaskViewModel
{
    public string Name { get; set; }
    public Priority Priority { get; set; }
    public string Description { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentNullException(Name, "Вкажіть назву завдання");
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentNullException(Description, "Вкажіть опис завдання");
        }
    }
}