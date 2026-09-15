namespace Day05Practice.Services;

public class MockTodoService : ITodoService
{
    public async Task<List<TodoItem>> GetAllAsync()
    {
        // Simulate network latency (500ms delay)
        await Task.Delay(500);

        return
        [
            new TodoItem(1, "Review Fluxor effects documentation", false),
            new TodoItem(2, "Complete Day 4 practice exercises", true),
            new TodoItem(3, "Read BYSResult Pattern Guide", false)
        ];
    }
}
