using System;
using System.Globalization;
using System.Windows;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for TaskEditWindow.xaml
    /// </summary>
    public partial class TaskViewWindow : Window
    {
        public TaskViewWindow(QTask task)
        {
            InitializeComponent();

            // Nahlad ulohy
            if (task != null)
            {
                TextBoxTaskViewName.Text = task.Title;
                TextBoxTaskViewDescription.Text = task.Description;
                try
                {
                    TextBoxTaskViewResponsibleUser.Text = task.ResponsibleUser.Name;
                }
                catch (Exception)
                {
                    // ignored 
                }
                try
                {
                    TextBoxTaskViewAssignedUser.Text = task.AssignedUser.Name;
                }
                catch (Exception)
                {
                    // ignored
                }
                try
                {
                    TextBoxTaskViewProject.Text = task.Project.Name;
                }
                catch (Exception)
                {
                    // ignored
                }
                CheckBoxTaskViewIsPrivate.IsChecked = task.IsPrivate;
                TextBoxTaskViewDeadline.Text = task.EstimatedEnd.ToString();
                TextBoxTaskViewNameCreated.Text = task.Created.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void ButtonTaskViewCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}