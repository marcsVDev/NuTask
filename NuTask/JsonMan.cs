using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NuTask.Routines;

namespace NuTask.JsonMan;

sealed class JsonManClass
{
    private string _pathJson ;
    public JsonManClass()
    {
        try
        {
            _pathJson = GetJsonFilePath();
            
            if (!File.Exists(_pathJson) || new FileInfo(_pathJson).Length == 0)
            {
                string dirPath = GetJsonDirectory();

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                File.WriteAllText(_pathJson, "[]");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in File path \n" + e.Message);
            throw;
        }
    }
    public void CreateANewRoutine(Routine rt)
    {
        List<Routine> rtList;
        
        if (File.ReadAllText(_pathJson) != "")
        {
            rtList = ReadDataRoutines();
            rtList.Add(rt);
        }
        else
        {
            rtList = new List<Routine> { rt };
        }

        rtList = OrganizeDateRoutines(rtList);
        using (StreamWriter writer = new StreamWriter(_pathJson, false))
        {
            string newJson = JsonSerializer.Serialize(rtList, new JsonSerializerOptions { WriteIndented = true });
            writer.Write(newJson);
            writer.Close();
        }
    }
    public void ResetSubscribe(List<Routine> rt)
    {
        List<Routine> rtList = OrganizeDateRoutines(rt);
        using (StreamWriter writer = new StreamWriter(_pathJson, false))
        {
            string newJson = JsonSerializer.Serialize(rtList, new JsonSerializerOptions { WriteIndented = true });
            writer.Write(newJson);
            writer.Close();
        }
    }
    private List<Routine> OrganizeDateRoutines(List<Routine> lastList)
    {
        List<DateTime> dates = lastList.Select(rt => rt.GetDate()).Cast<DateTime>().ToList();
        List<DateTime> sortedDates = dates.OrderBy(date => date).ToList();
        List<Routine> actualList = new List<Routine>();

        foreach (var date in sortedDates)
        {
            foreach (var rt in lastList)
            {
                if (rt.GetDate() == date)
                {
                    actualList.Add(rt);
                    break;
                }
            }
        }
        return actualList;
    }
    public List<Routine> ReadDataRoutines()
    {
        try
        {
            List<Routine> rtList;
            using (StreamReader streamReader = new StreamReader(_pathJson))
            {
                string json = streamReader.ReadToEnd();
                rtList = JsonSerializer.Deserialize<List<Routine>>(json) ?? new List<Routine>();
                streamReader.Close();
            }
            return rtList;
        }
        catch (Exception e)
        {
            File.Delete(_pathJson);
            new JsonManClass();
        }
        return null;
    }

    public bool VerifyDateDiference(DateTime date)
    {
        List<Routine> rtList = ReadDataRoutines();
        foreach (var rt in rtList)
        {
            DateTime? rtDate = rt.GetDate();
            if (rtDate.Value.ToString("dd/MM/yyyy") == date.Date.ToString("dd/MM/yyyy"))
                return false;
        }
        return true;
    }
    private string GetJsonFilePath()
    {
        string fileName = "data.json";
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var jsonFilePath = Path.Combine(appDataDir, "NuTask", fileName);
        return jsonFilePath;
    }
    private string GetJsonDirectory()
    {
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var jsonFilePath = Path.Combine(appDataDir, "NuTask");
        return jsonFilePath;
    }
}