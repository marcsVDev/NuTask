using System;
using System.Collections.Generic;
using System.IO;
using NuTask.Routines.Tasks;
using NuTask.Routines;
using NuTask.JsonMan;
using NuTask.Views;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Media.Imaging;

namespace NuTask.RoutineManager;

class MainManager
{
    private JsonManClass jsonMan;

    public MainManager()
    {
        jsonMan = new JsonManClass();
    }
    public void CallBaseRoutine(StackPanel stackPnn)
    {
        var actualRoutines = jsonMan.ReadDataRoutines();
        if (VerifyListRoutines(actualRoutines))
        {
            for (int i = 0; i < actualRoutines.Count; i++)
            {
                var rt = actualRoutines[i];
                Border border = CreateAInTaskMan(
                    i,
                    rt.GetDate(),
                    rt.GetName(),
                    rt.GetTasksList(),
                    (bool)rt.GetPriority()
                );
                stackPnn.Children.Add(border);
            }
        }
    }

    private bool VerifyListRoutines(List<Routine> rtS) => rtS.Count > 0;

    private Border CreateAInTaskMan(int index,DateTime? date, string? name, List<Task> tasksList, bool priority)
    {
        List<TextBlock> txtBlocks = new List<TextBlock>();
        int thicknessSpace = 0;
        foreach (var tL in tasksList)
        {
            TextBlock txtB = new TextBlock();
            txtB.Margin = new Thickness(0,thicknessSpace,0,0);
            txtB.HorizontalAlignment = HorizontalAlignment.Left;
            txtB.VerticalAlignment = VerticalAlignment.Top;
            txtB.Text = $"{tL.GetID()}:{tL.GetStrTask()}";
            txtBlocks.Add(txtB);
            thicknessSpace += 15;
        }
        var pnnTasks = new Panel();
        pnnTasks.Margin = new Thickness(0, 40, 0, 0);
        pnnTasks.Background = new SolidColorBrush(Color.Parse("#3A3632"));
        foreach (var tL in txtBlocks)
        {
            pnnTasks.Children.Add(tL);
        }
        
        var delBtt = new Button
        {
            Name = "DeleteRoutine",
            Tag = index,
            Foreground = new SolidColorBrush(Color.Parse("#FFFFFF")),
            Content = new Image
            {
                Width = 20,
                Height = 20,
                Source = GetFileRequest.GetImageFilesBitMap("trash")
            }
        };
        delBtt.Click += DelBtt;

        var editBtt = new Button
        {
            Name = "EditRoutine",
            Tag = index,
            Foreground = new SolidColorBrush(Color.Parse("#FFFFFF")),
            Content = new Image
            {
                Width = 20,
                Height = 20,
                Source = GetFileRequest.GetImageFilesBitMap("edit")
            }
        };
        editBtt.Click += EditBtt;
        string dateStr = date.Value.Date.ToString("dd/MM/yyyy");
        var mainBorder = new Border
        {
            Name = "BorderTaskMan",
            Background = new SolidColorBrush(Color.Parse("#3A3632")),
            BorderThickness = new Thickness(10),
            CornerRadius = new CornerRadius(5),
            Child = new Panel
            {
                Background = new SolidColorBrush(Color.Parse("#3A3632")),
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Date: {dateStr}",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    },
                    new TextBlock
                    {
                        Text = $"Name: {name}",
                        Margin = new Thickness(0, 18, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    },
                    new Border
                    {
                        Background = new SolidColorBrush(Color.Parse("#FF0000")),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        BorderThickness = new Thickness(4),
                        CornerRadius = new CornerRadius(4),
                        IsVisible = priority,
                        Child = new Image
                        {
                            Width = 20,
                            Height = 20,
                            Source = GetFileRequest.GetImageFilesBitMap("alert")
                        }
                    },
                    new Border
                    {
                        Margin = new Thickness(0, 0, 25, 0),
                        Background = new SolidColorBrush(Color.Parse("#3A3632")),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        BorderThickness = new Thickness(2),
                        CornerRadius = new CornerRadius(4),
                        Child = delBtt
                    },
                    new Border
                    {
                        Name = "EditRoutine",
                        Tag = index,
                        Margin = new Thickness(0, 0, 60, 0),
                        Background = new SolidColorBrush(Color.Parse("#3A3632")),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        BorderThickness = new Thickness(2),
                        CornerRadius = new CornerRadius(4),
                        Child = editBtt
                    },
                    pnnTasks
                }
            }
    };
    return mainBorder;
    }

    public void DelBtt(object sender, RoutedEventArgs args)
    {
        if (sender is Button deleteButton)
        {
            deleteButton.IsEnabled = false;
            int index = (int)deleteButton.Tag;
            var rtList = jsonMan.ReadDataRoutines();
            rtList.RemoveAt(index);
            jsonMan.ResetSubscribe(rtList);
            MainWindow.ResetMain();
        }
    }
    public void EditBtt(object sender, RoutedEventArgs args)
    {
        if (sender is Button editButton)
        {
            int index = (int)editButton.Tag;
            var rtList = jsonMan.ReadDataRoutines();
            var winEdit = new EditWindow(rtList[index], (int)editButton.Tag);
            winEdit.Show();
        }
    }
}