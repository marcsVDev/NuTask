using System.Collections.Generic;

namespace NuTask.Routines.Tasks;

public sealed class Task
{
    public int ID{get; set;}
    public string StrTask{ get; set;}
    public bool Check { get; set;}

    public Task(int id, string strTask, bool check)
    {
        ID = id;
        StrTask = strTask;
    }
}
public static class TaskMethods
{
    public static string GetStrTaskByID(this List<Task> value, int id)
    {
        for (int i = 0; i< value.Count;i++)
        {
            if (value[i].GetID() == id)
            {
                return value[i].StrTask;
            }
        }
        return "";
    }
    public static bool IsChecked(this Task value)
    {
        return value.Check;
    }
    public static int GetID(this Task value)
    {
        return value.ID;
    }
    public static string GetStrTask(this Task value)
    {
        return value.StrTask;
    }
}