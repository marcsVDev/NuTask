using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using NuTask.JsonMan;
using NuTask.Routines;
using NuTask.Routines.Tasks;
using NuTask.RoutineManager;

namespace NuTask.Views;

public partial class MainWindow : Window
{
    private static StackPanel StackPnnTManager = StackPnnTManager = new StackPanel() {
        Margin = new Thickness(-5,0,-5,5)
    };
    private int taskNum = 1;
    private List<TextBox> TTxtBoxList = new List<TextBox>();
    private List<Label> TLabelList = new List<Label>();
    public MainWindow()
    {
        InitializeComponent();
        InitializeAppGeneral();
        if (ScrollViewOfManager.Content == null)
        {
            ScrollViewOfManager.Content = StackPnnTManager;
        }
    }

    private void InitializeAppGeneral()
    {
        
        
        var jsonMain = new JsonManClass();
        var rtListLater = jsonMain.ReadDataRoutines();
        var rtList = new List<Routine>();
        bool today = false;
        
        foreach (var rt in rtListLater)
        {
            if(rt.GetDate() > DateTime.Now || rtListLater[0].GetDate()?.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy"))
                rtList.Add(rt);
        }
        jsonMain.ResetSubscribe(rtList);
        
        if (rtList.Count > 0 && rtList[0].GetDate().Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy"))
        {
            today = true;
            var rtNow = rtList[0];
            TodayName.Text = $"Name: {rtNow.GetName()}";
            TodayPriority.IsVisible = (bool)rtNow.GetPriority();
            TodayDate.Text = $"Date: {rtNow.GetDate().Value.Date.ToString("dd/MM/yyyy")}";
            var taskList = rtNow.GetTasksList();
            STckPnnTasksToday.Children.Clear();
            
            foreach (var task in taskList)
            {
                var checkBox = new CheckBox
                {
                    IsChecked = task.IsChecked(),
                    Tag = task.GetID(),
                    Content = new TextBlock
                    {
                        Text = task.GetStrTask()
                    }
                };
                STckPnnTasksToday.Children.Add(checkBox);
            }
        }
        else
        {
            TodayName.Text = "Any routine today!";
            STckPnnTasksToday.Children.Clear();
            TodayDate.IsVisible = false;
            TodayPriority.IsVisible = false;
        }
        int indexNow = today ? 1 : 0;
        if (rtList.Count > 0 && indexNow < rtList.Count && rtList[indexNow] != null)
        {
            var rtNext = rtList[indexNow];
            NextName.Text = $"Name: {rtNext.GetName()}";
            NextPriority.IsVisible = (bool)rtNext.GetPriority();
            NextDate.Text = $"Date: {rtNext.GetDate().Value.Date.ToString("dd/MM/yyyy")}";
            NextDate.IsVisible = true;
            var taskList = rtNext.GetTasksList();
            STckPnnTasksNext.Children.Clear();
            foreach (var task in taskList)
            {
                var txtBlock = new TextBlock
                {
                    Text = task.GetStrTask()
                };
                STckPnnTasksNext.Children.Add(txtBlock);
            }
        }
        else
        {
            NextName.Text = "Any routine for next!";
            STckPnnTasksNext.Children.Clear();
            NextDate.IsVisible = false;
            NextPriority.IsVisible = false;
        }
        
    }
    private void GeneralItemPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TabItem)
            InitializeAppGeneral();
    }
    private void ManagerItemPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TabItem)
            ResetMain();
    }
    private async void AddATask(object sender, RoutedEventArgs args)
    {
        if (taskNum >= 50)
        {
            AddTBtt.IsEnabled = false;
            AddTBtt.Content = "Max limit of tasks!";
            await System.Threading.Tasks.Task.Delay(1000);
            AddTBtt.IsEnabled = true;
            AddTBtt.Content = "Add new task";
            return;
        }

        Label lb = new Label() {
            Content = taskNum,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,            
        };
        TextBox txtBox = new TextBox(){
            Name = $"T{taskNum}",
            Text = $"Task {taskNum}",
        };
        TTxtBoxList.Add(txtBox);
        TLabelList.Add(lb);

        Grid.SetColumn(lb,0);
        Grid.SetColumn(txtBox,1);

        int n = taskNum-1;
        TaskAGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetRow(lb,n);
        Grid.SetRow(txtBox,n);

        TaskAGrid.Children.Add(txtBox);
        TaskAGrid.Children.Add(lb);

        taskNum++;
    }
    private void DeleteTask(object sender, RoutedEventArgs args)
    {
        if(TTxtBoxList.Count > 0 && TLabelList.Count > 0)
        {
            TaskAGrid.Children.Remove(TTxtBoxList[^1]);
            TTxtBoxList.Remove(TTxtBoxList[^1]);
            TaskAGrid.Children.Remove(TLabelList[^1]);
            TLabelList.Remove(TLabelList[^1]);
            taskNum--;
        }
    }
    private void ConfirmRoutine(object sender, RoutedEventArgs args)
    {
        JsonManClass jsonMan = new JsonManClass();
        if (!CheckUserInputStats())
        {
            TaskMessage("Some value is missing");
            return;
        }
        if (!jsonMan.VerifyDateDiference((DateTime)DatePicker.SelectedDate))
        {
            TaskMessage("That date already exists");
            return;
        }
        if (DatePicker.SelectedDate < DateTime.Now)
        {
            TaskMessage("Old date");
            return;
        } 
        
        var name = RoutineName.Text;
        var priority = TogglePriority.IsChecked;
        var dateTime = DatePicker.SelectedDate; 
        var taskList = new List<Task>();        

        for(int i = 0; i < TLabelList.Count;i++)
        {
            var task = new Task(i, TTxtBoxList[i].Text+"",false);
            taskList.Add(task);
        }
        var rtActual  = new Routine(            
            name,
            dateTime,
            priority,
            taskList           
        );
        jsonMan.CreateANewRoutine(rtActual);
        TaskMessage("Added");
    }
    private async void TaskMessage(string textOfShow)
    {
        CRoutineBtt.Content = textOfShow;
        CRoutineBtt.IsEnabled = false;
        await System.Threading.Tasks.Task.Delay(1000);
        CRoutineBtt.Content = "Confirm routine";
        CRoutineBtt.IsEnabled = true;
    }
    private bool CheckUserInputStats() => TLabelList.Count > 0 && !string.IsNullOrEmpty(RoutineName.Text) && DatePicker.SelectedDate != null ? true : false;
    private void DeleteAllTasks(object sender, RoutedEventArgs args)
    {
        if (TTxtBoxList.Count < 1 && TLabelList.Count < 1)
            return;
        
        TaskAGrid.Children.Clear();
        TTxtBoxList.Clear();
        TLabelList.Clear();
        taskNum = 1;
    }
    public static void ResetMain()
    {
        StackPnnTManager.Children.Clear();
        var callMan = new MainManager();
        callMan.CallBaseRoutine(StackPnnTManager);
    }
}