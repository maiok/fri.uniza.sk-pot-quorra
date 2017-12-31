using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quorra.Library;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly QuorraContext _dbContext;
        private List<QUser> UserList { get; set; }
        private List<QProject> ProjectList { get; set; }
        private List<QTask> TaskList { get; set; }
        private QUser _selectedUser;
        private QProject _selectedProject;
        private QTask _selectedTask;

        public MainWindow()
        {
            InitializeComponent();

            _dbContext = new QuorraContext();

            UserList = new List<QUser>();
            ProjectList = new List<QProject>();
            TaskList = new List<QTask>();
            _selectedUser = null;
        }

        public void RefreshListUsers(List<QUser> userList)
        {
            UserList = userList;
            try
            {
                ListViewUsers.ItemsSource = UserList;
            }
            catch (Exception)
            {
                // ignored
            }
            // Aktualizujem pocty
            RefreshUserFilteredCounts();
            // Deaktivujem tlacidla akcii
            ButtonEditUser.IsEnabled = false;
            ButtonRemoveUser.IsEnabled = false;
        }

        public void RefreshListProjects(List<QProject> projectList)
        {
            ProjectList = projectList;
            try
            {
                ListViewProjects.ItemsSource = ProjectList;
            }
            catch (Exception)
            {
                // ignored
            }
            // Aktualizujem pocty
            RefreshProjectFilteredCounts();
            // Deaktivujem tlacidla akcii
            ButtonEditProject.IsEnabled = false;
            ButtonRemoveProject.IsEnabled = false;
        }

        public void RefreshListTasks(List<QTask> taskList)
        {
            TaskList = taskList;
            try
            {
                ListViewTasks.ItemsSource = TaskList;
            }
            catch (Exception)
            {
                // ignored
            }
            RefreshTaskFilteredCounts();
            ButtonEditTask.IsEnabled = false;
            ButtonRemoveTask.IsEnabled = false;
        }

        public void RefreshUserFilteredCounts()
        {
            TextBlockUsersFiltered.Text = UserList.Count.ToString();
            TextBlockUsersCount.Text = _dbContext.GetUsers().Count().ToString();
        }

        public void RefreshProjectFilteredCounts()
        {
            TextBlockFilteredProjects.Text = ProjectList.Count.ToString();
            TextBlockFilterdProjectsAll.Text = _dbContext.GetProjects().Count().ToString();
        }

        public void RefreshTaskFilteredCounts()
        {
            TextBlockFilteredTasks.Text = TaskList.Count.ToString();
            TextBlockFilteredTasksAll.Text = _dbContext.GetTasks().Count().ToString();
        }

        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserWindow(_dbContext, this, null);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Obsluha prepinania tabov, kedy po kazdom prepnuti sa aktualizuje prislusny zoznam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Tu mi chyta focus ked kliknem hocikde v ramci TabControl, preto rozlisujem konkretne, kde som klikol
            // Prepinam taby?
            try
            {
                var tabName = ((System.Windows.FrameworkElement) e.AddedItems[0]).Name;
                switch (tabName)
                {
                    case "TabItemUsers":
                    {
                        RefreshListUsers(_dbContext.GetUsers().ToList());
                        break;
                    }
                    case "TabItemProjects":
                    {
                        RefreshListProjects(_dbContext.GetProjects().ToList());
                        break;
                    }
                    case "TabItemTasks":
                    {
                        RefreshListTasks(_dbContext.GetTasks().ToList());
                        break;
                    }
                    case "TabItemMessenger":
                    {
                        // todo
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ListViewTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // todo otvorit vsetky udaje o ulohe
        }

        private void ButtonNewProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectWindow(null, this, _dbContext);
            dialog.ShowDialog();
        }

        private void ListViewUsers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Nastavim kontext oznaceneho pouzivatela
            _selectedUser = (QUser) ListViewUsers.SelectedItem;
            // Zapnem si tlacidla pre Edit a Zmazanie
            ButtonEditUser.IsEnabled = true;
            ButtonRemoveUser.IsEnabled = true;
        }

        private void ButtonEditUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserWindow(_dbContext, this, _selectedUser);
            dialog.ShowDialog();
        }

        private void ButtonRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show($@"Ste si istý, že chcete zmazať používateľa {_selectedUser.Name}?",
                    "Zmazanie používateľa", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                    MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    _dbContext.RemoveUser(_selectedUser);
                    RefreshListUsers(_dbContext.GetUsers().ToList());
                    // Po vymazani je potrebne znova vypnut tlacidla Remove a Edit
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Nastala neočakávaná chyba a používateľ nemohol byť vymazaný!");
                    Console.WriteLine(exception);
                }
            }
        }

        private void ButtonApplyFilterUsers_Click(object sender, RoutedEventArgs e)
        {
            var userName = (TextBoxFilterUserName.Text.Trim() != "") ? TextBoxFilterUserName.Text : null;

            // Tu si nastavim pole hodnot, ktore boli zaskrnute
            List<QUserRole> userRoles = new List<QUserRole>();
            if ((bool) CheckBoxBackendDeveloper.IsChecked)
            {
                userRoles.Add(QUserRole.BackendDeveloper);
            }
            if ((bool) CheckBoxFrontendDeveloper.IsChecked)
            {
                userRoles.Add(QUserRole.FrontendDeveloper);
            }
            if ((bool) CheckBoxProductOwner.IsChecked)
            {
                userRoles.Add(QUserRole.ProductOwner);
            }
            if ((bool) CheckBoxProjectManager.IsChecked)
            {
                userRoles.Add(QUserRole.ProjectManager);
            }
            if ((bool) CheckBoxSystemAdmin.IsChecked)
            {
                userRoles.Add(QUserRole.SystemAdministrator);
            }
            if ((bool) CheckBoxTester.IsChecked)
            {
                userRoles.Add(QUserRole.Tester);
            }

            // Aktualizujem vsetky potrebne hodnoty
            UserList = _dbContext.ApplyFilterUsers(userName, userRoles);
            RefreshListUsers(UserList);
        }

        private void ListViewUsers_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewUsers_SelectionChanged(sender, null);
            ButtonEditUser_Click(sender, null);
        }

        private void ListViewProjects_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewProjects_SelectionChanged(sender, null);
            ButtonEditProject_Click(sender, null);
        }

        private void ListViewProjects_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedProject = (QProject) ListViewProjects.SelectedItem;
            ButtonEditProject.IsEnabled = true;
            ButtonRemoveProject.IsEnabled = true;
        }

        private void ButtonEditProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectWindow(_selectedProject, this, _dbContext);
            dialog.Show();
        }

        private void ButtonRemoveProject_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    $@"Ste si istý, že chcete zmazať projekt {
                            _selectedProject.Name
                        }? Zmažú sa aj všetky úlohy priradené k tomuto projektu!",
                    "Zmazanie používateľa", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                    MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    _dbContext.RemoveProject(_selectedProject);
                    RefreshListProjects(_dbContext.GetProjects().ToList());
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Nastala neočakávaná chyba a používateľ nemohol byť vymazaný!");
                    Console.WriteLine(exception);
                }
            }
        }

        private void ButtonApplyFilterProjects_Click(object sender, RoutedEventArgs e)
        {
            var projectName = (TextBoxFilterProjectName.Text.Trim() != "") ? TextBoxFilterProjectName.Text : null;
            var productOwnerName = (TextBoxFilterOwnerName.Text.Trim() != "") ? TextBoxFilterOwnerName.Text : null;
            var estimatedEndFrom = DatePickerFilterEndFrom.SelectedDate;
            var estimatedEndTo = DatePickerFilterEndTo.SelectedDate;

            ProjectList =
                _dbContext.ApplyFilterProjects(projectName, productOwnerName, estimatedEndFrom, estimatedEndTo);
            RefreshListProjects(ProjectList);
        }

        private void ListViewTasks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedTask = (QTask) ListViewTasks.SelectedItem;
            ButtonEditTask.IsEnabled = true;
            ButtonRemoveTask.IsEnabled = true;
        }
    }
}