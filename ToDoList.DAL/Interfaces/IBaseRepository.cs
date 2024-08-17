namespace ToDoList.DAL.Interfaces;

//Додавання дженерик інтерфейсу, який буде реалізовувати CRUD операції

public interface IBaseRepository<T>
{
    /// <summary>
    ///  Операція додавання сутності до БД
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task Create(T entity);

    //Методд отримання всіх записів з БД
    IQueryable<T> GetAll();

    //Видалення об'єкту з БД
    Task Delete(T entity);

    //Оновлення об'єкту в БД
    Task<T> Update(T entity);
}