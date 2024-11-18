using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NuTask.Routines;
using NuTask.JsonMan;

namespace NuTask.Views;

public class EditWindow : Window
{

    private TextBox txtBox = new TextBox
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        
    };
    
    // Date Picker for Routine Date
    private CalendarDatePicker datePicker = new CalendarDatePicker
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Bottom,
        
    };
    
    // Priority Border Indicator
    private Border priorityBorder = new Border
    {
        Background = new SolidColorBrush(Color.Parse("#FF0000")),
        HorizontalAlignment = HorizontalAlignment.Right,
        Margin = new Thickness(0, -30, 5, 0),
        BorderThickness = new Thickness(2),
        CornerRadius = new CornerRadius(4),
        
    };

    private Image imageAlert = new Image
    {
        Height = 20,
        Width = 20,
        Source = GetFileRequest.GetImageFilesBitMap("alert")
    };

    private CheckBox checkBox = new CheckBox
    {
        HorizontalAlignment = HorizontalAlignment.Right,
        Margin = new Thickness(0, -30, 20, 0)
    };
    

    private Button confirmButton = new Button
    {
        HorizontalAlignment = HorizontalAlignment.Right,
        Margin = new Thickness(5),
        Content = "Confirm"
    };
    

    private StackPanel stckWin = new StackPanel
    {
        Background = new SolidColorBrush(Color.Parse("#8F7457"))
    };
        
    public EditWindow(Routine rt, int tag)
    {
        CanResize = false;
        Width = 200;
        Height = 110;
        Title = "Edit Routine";
        
        confirmButton.Click += ConfirmEdit;
        ToolTip.SetTip(checkBox, "Priority");
        priorityBorder.Child = imageAlert;
        
        txtBox.Text = rt.GetName();
        datePicker.SelectedDate = rt.GetDate();
        checkBox.IsChecked = (bool)rt.GetPriority();
        priorityBorder.IsVisible = (bool)rt.GetPriority();
        confirmButton.Tag = tag;
        
        stckWin.Children.Add(txtBox);
        stckWin.Children.Add(datePicker);
        stckWin.Children.Add(priorityBorder);
        stckWin.Children.Add(checkBox);
        stckWin.Children.Add(confirmButton);

        Content = stckWin;

        Deactivated += (sender, e) => Close();
    }

    private void ConfirmEdit(object sender, RoutedEventArgs args)
    {
        if (sender is Button actualBtt)
        {
            int tagIndex = (int)actualBtt.Tag;
            var jsonMain = new JsonManClass();
            if (GetRoutineEdited(tagIndex) == null)
            {
                ButtonMessage(actualBtt, "Some empty value");
            }
            if (datePicker.SelectedDate <= DateTime.Now)
            {
                ButtonMessage(actualBtt,"Old date");
                return;
            }
            Routine actualRt = GetRoutineEdited(tagIndex);
            if (!jsonMain.VerifyDateDiference((DateTime)datePicker.SelectedDate))
            {
                if (datePicker.SelectedDate.Value.ToString("dd/MM/yyyy") != actualRt.GetDate().Value.ToString("dd/MM/yyyy"))
                {
                    ButtonMessage(actualBtt, "That date already exists");
                    return;
                }
            }
            
            Routine rt = new Routine(
                txtBox.Text,
                datePicker.SelectedDate,
                checkBox.IsChecked,
                actualRt.GetTasksList()
            );
            var actualRtList = jsonMain.ReadDataRoutines();
            actualRtList[tagIndex] = rt;
            jsonMain.ResetSubscribe(actualRtList);
        
            actualBtt.IsEnabled = false;
            MainWindow.ResetMain();
            Close();
        }
    }
    private async void ButtonMessage(Button actualBtt,string textOfShow)
    {
        actualBtt.Content = textOfShow;
        actualBtt.IsEnabled = false;
        await System.Threading.Tasks.Task.Delay(1000);
        actualBtt.Content = "Confirm routine";
        actualBtt.IsEnabled = true;
    }
    private Routine GetRoutineEdited(int index)
    {
        var jsonMain = new JsonManClass();
        if (!string.IsNullOrEmpty(txtBox.Text) &&
            datePicker.SelectedDate.HasValue &&
            index >= 0 &&
            index < jsonMain.ReadDataRoutines().Count)
        {
            return jsonMain.ReadDataRoutines()[index];
        }
        return null;
    }
}