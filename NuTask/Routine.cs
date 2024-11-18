using System;
using System.Collections.Generic;
using NuTask.Routines.Tasks;

namespace NuTask.Routines;

public class Routine
{
    public string? Name { get; set;}
    public DateTime? Date { get; set;}
    public bool? Priority{get;set;}
    public List<Task> Tasks{get;set;}

    public Routine(string? name, DateTime? date, bool? priority, List<Task> tasks)
    {
        Name = name;
        Date = date;
        Priority = priority;
        Tasks = tasks;
    }

}
public static class RoutineExtensions
{
    public static string? GetName(this Routine value) => value.Name;
    public static DateTime? GetDate(this Routine value) => value.Date;
    public static bool? GetPriority(this Routine value) => value.Priority;
    public static List<Task> GetTasksList(this Routine value) => value.Tasks;
}