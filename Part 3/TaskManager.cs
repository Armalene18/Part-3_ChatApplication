using System.Collections.Generic;
using System.Linq;

public class TaskManager
{
    private List<TaskItem> tasks = new();

    public void AddTask(string title, string description)
    {
        tasks.Add(new TaskItem { Title = title, Description = description });
    }

    public List<TaskItem> GetPendingTasks() => tasks.Where(t => !t.IsCompleted).ToList();
    public List<TaskItem> GetAllTasks() => tasks;
}
