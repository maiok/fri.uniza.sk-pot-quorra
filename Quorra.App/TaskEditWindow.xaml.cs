using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for TaskEditWindow.xaml
    /// </summary>
    public partial class TaskEditWindow : Window
    {
        private readonly QuorraContext _dbContext;
        private readonly MainWindow _mainWindow;
        private readonly QTask _task;

        public readonly List<QUser> UsersList;
        public readonly List<QProject> ProjectList;

        public TaskEditWindow(QuorraContext dbContext, MainWindow mainWindow, QTask task)
        {
            InitializeComponent();

            _dbContext = dbContext;
            _mainWindow = mainWindow;
            _task = task;

            // Pociatocne naplnenie comboboxov

            UsersList = _dbContext.GetUsers().ToList();
            ProjectList = _dbContext.GetProjects().ToList();

            // Zodpovedna osoba
            ComboBoxFormTaskResponsibleUser.SelectedValuePath = "Id";
            ComboBoxFormTaskResponsibleUser.DisplayMemberPath = "Name";
            ComboBoxFormTaskResponsibleUser.ItemsSource = UsersList;
            // Priradena osoba
            ComboBoxFormTaskAssignedUser.SelectedValuePath = "Id";
            ComboBoxFormTaskAssignedUser.DisplayMemberPath = "Name";
            ComboBoxFormTaskAssignedUser.ItemsSource = UsersList;
            // Projekty
            ComboBoxFormTaskProject.SelectedValuePath = "Id";
            ComboBoxFormTaskProject.DisplayMemberPath = "Name";
            ComboBoxFormTaskProject.ItemsSource = ProjectList;

            // Editacia ulohy
            if (_task != null)
            {
                TextBoxFormTaskName.Text = _task.Title;
                TextBoxFormTaskDescription.Text = _task.Description;
                ComboBoxFormTaskResponsibleUser.SelectedItem = _task.ResponsibleUser;
                ComboBoxFormTaskAssignedUser.SelectedItem = _task.AssignedUser;
                ComboBoxFormTaskProject.SelectedItem = _task.Project;
                CheckBoxFormTaskIsPrivate.IsChecked = _task.IsPrivate;
                DatePickerFormTaskDeadline.SelectedDate = _task.EstimatedEnd;
            }
        }

        private void ButtonFormTaskCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonFormTaskOk_Click(object sender, RoutedEventArgs e)
        {
            var taskTitle = TextBoxFormTaskName.Text.Trim();
            var taskDescription = TextBoxFormTaskDescription.Text.Trim();
            var taskResponsibleUser = (QUser) ComboBoxFormTaskResponsibleUser.SelectedItem;
            var taskAssignedUser = (QUser) ComboBoxFormTaskAssignedUser.SelectedItem;
            var taskProject = (QProject) ComboBoxFormTaskProject.SelectedItem;
            var taskIsPrivate = CheckBoxFormTaskIsPrivate.IsChecked != null && (bool) CheckBoxFormTaskIsPrivate.IsChecked;
            var taskEstimatedEnd = DatePickerFormTaskDeadline.SelectedDate;

            if (taskTitle == "")
            {
                MessageBox.Show("Názov úlohy je povinný!");
            }
            else
            {
                // Pokial nemam nastaveny task, idem vytvorit novy
                if (_task == null)
                {
                    QTask task = new QTask
                    {
                        Title = taskTitle,
                        Description = taskDescription,
                        ResponsibleUserId = taskResponsibleUser?.Id ?? null,
                        AssignedUserId = taskAssignedUser?.Id ?? null,
                        ProjectId = taskProject?.Id ?? null,
                        IsPrivate = taskIsPrivate,
                        EstimatedEnd = taskEstimatedEnd
                    };

                    _dbContext.InsertTask(task);
                }
                // Ak mam nestavenu ulohu, idem editovat
                else
                {
                    _task.Title = taskTitle;
                    _task.Description = taskDescription;
                    _task.ResponsibleUserId = taskResponsibleUser?.Id ?? null;
                    _task.AssignedUserId = taskAssignedUser?.Id ?? null;
                    _task.ProjectId = taskProject?.Id ?? null;
                    _task.IsPrivate = taskIsPrivate;
                    _task.EstimatedEnd = taskEstimatedEnd;

                    _dbContext.UpdateTask(_task);
                }
                _mainWindow.RefreshListTasks(_dbContext.GetTasks().ToList(), false);
                Close();
            }
        }
    }
}