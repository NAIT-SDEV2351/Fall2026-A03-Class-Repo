namespace Day05Practice.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync();
}
