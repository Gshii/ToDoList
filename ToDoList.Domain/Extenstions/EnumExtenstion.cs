using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ToDoList.Domain.Extenstions;

public static class EnumExtenstion
{
    public static string GetDisplayName(this System.Enum enumValue)
    {
        return enumValue.GetType().GetMember(enumValue.ToString())
            .First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? "Не визначений";
    }
}