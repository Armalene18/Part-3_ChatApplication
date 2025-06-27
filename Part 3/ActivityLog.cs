using System;
using System.Collections.Generic;

public class ActivityLog
{
    private List<string> log = new();

    public void Add(string action)
    {
        log.Add($"{DateTime.Now:t} - {action}");
    }

    public List<string> GetLog() => log.TakeLast(10).ToList();
}
