using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Quorra.Library;
using Quorra.Model;
using Message = Quorra.Library.Message;
using MessageBox = System.Windows.MessageBox;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuorraContext _dbContext;
        public List<QUser> UserList { get; set; }
        public List<QProject> ProjectList { get; set; }
        public List<QTask> TaskList { get; set; }
        private List<string> DataToUser { get; set; }
        private List<string> OnlineUsers { get; set; }

        private QUser _selectedUser;
        private QProject _selectedProject;
        private QTask _selectedTask;
        private string _loggedUser = null;
        private bool _firstEntryToMessenger = false;

        private static DuplexChannelFactory<IChatService> _channelFactory;
        private static IChatService _server;

        public MainWindow()
        {
            InitializeComponent();

            _dbContext = new QuorraContext();

            UserList = new List<QUser>();
            ProjectList = new List<QProject>();
            TaskList = new List<QTask>();
            DataToUser = new List<string>();
            OnlineUsers = new List<string>();

            _selectedUser = null;

            // Naplnil si do filtra stavy sukromnych sprav
            ComboBoxFilterTaskPrivate.Items.Add(new PrivateItem("Nezáleží", 2));
            ComboBoxFilterTaskPrivate.Items.Add(new PrivateItem("Je", 1));
            ComboBoxFilterTaskPrivate.Items.Add(new PrivateItem("Nie je", 0));

            // Pripojenie klienta na sluzbu chatu
            _channelFactory = new DuplexChannelFactory<IChatService>(new ClientCallback(), "ChatServiceEndPoint");
            _server = _channelFactory.CreateChannel();

            // Akcia po zavreti okna
            Closing += new CancelEventHandler(MainWindow_Closing);
        }

        /// <summary>
        /// Ked zavriem okno a je prihlaseny uzivatel do chatu, uvolnim aby vznikol priestor pre dalsie prihlasenia
        /// ZDROJ - https://stackoverflow.com/questions/3683450/handling-the-window-closing-event-with-wpf-mvvm-light-toolkit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            UserLogout();
        }

        /// <summary>
        /// Metoda mi obsluhuje odhlasenie pouzivatela
        /// </summary>
        private bool UserLogout()
        {
            if (_loggedUser != null)
            {
                try
                {
                    return _server.Logout(_loggedUser);
                }
                catch (Exception)
                {
                    MessageBox.Show("Neočakávana chyba na serveri. Užívateľ nebol uvoľnený.");
                }
            }
            return false;
        }

        public void RefreshListUsers(List<QUser> userList, bool deactivateButtons)
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
            if (deactivateButtons)
            {
                ButtonEditUser.IsEnabled = false;
                ButtonRemoveUser.IsEnabled = false;
            }
        }

        public void RefreshListProjects(List<QProject> projectList, bool deactivateButtons)
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
            if (deactivateButtons)
            {
                ButtonEditProject.IsEnabled = false;
                ButtonRemoveProject.IsEnabled = false;
            }
        }

        public void RefreshListTasks(List<QTask> taskList, bool deactivateButtons)
        {
            TaskList = taskList;
            try
            {
                ListViewTasks.ItemsSource = TaskList;

                // Naplnim si combobox hodnoty do filtra
                // Priradena osoba
                ComboBoxFilterTaskAssigned.SelectedValuePath = "Id";
                ComboBoxFilterTaskAssigned.DisplayMemberPath = "Name";
                ComboBoxFilterTaskAssignedContainer.Collection = _dbContext.GetUsers().ToList();
                // Projekty
                ComboBoxFilterTaskProject.SelectedValuePath = "Id";
                ComboBoxFilterTaskProject.DisplayMemberPath = "Name";
                ComboBoxFilterProjectContainer.Collection = _dbContext.GetProjects().ToList();
            }
            catch (Exception)
            {
                // ignored
            }
            RefreshTaskFilteredCounts();
            if (deactivateButtons)
            {
                ButtonEditTask.IsEnabled = false;
                ButtonRemoveTask.IsEnabled = false;
            }
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

        /// <summary>
        /// Metoda mi aktualizuje zoznam prihlasenych pouzivatelov nacitanych zo servera
        /// </summary>
        private void RefreshOnlineChatUsers()
        {
            // Defaultne 0 - Vsetci
            var selectedIndex = (ComboBoxChatListUsers.SelectedIndex == -1) ? 0 : ComboBoxChatListUsers.SelectedIndex;
            // Combobox
            var usersPom = new List<string> {"Všetci"};
            usersPom.AddRange(_server.GetUserNames());
            DataToUser = usersPom;
            ComboBoxChatListUsers.ItemsSource = DataToUser;
            // Po refreshi vratim vyber na povodnu hodnotu
            ComboBoxChatListUsers.SelectedIndex = selectedIndex;
            // Online list
            var onlineUserPom = new List<string>();
            onlineUserPom.AddRange(_server.GetUserNames());
            OnlineUsers = onlineUserPom;
            ListViewChatOnlineUsers.ItemsSource = OnlineUsers;
        }

        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserWindow(_dbContext, this, null);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Metoda mi znovu vytvori kontext do databazy. Potrebujem to vtedy, kedy aktualizujem data a 
        /// potrebujem ich mat aktualizovane aj na ostatnych klientoch
        /// </summary>
        private void RecreateDbContext()
        {
            _dbContext = new QuorraContext();
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
                        RefreshListUsers(_dbContext.GetUsers().ToList(), false);
                        break;
                    }
                    case "TabItemProjects":
                    {
                        RefreshListProjects(_dbContext.GetProjects().ToList(), false);
                        break;
                    }
                    case "TabItemTasks":
                    {
                        RefreshListTasks(_dbContext.GetTasks().ToList(), false);
                        break;
                    }
                    case "TabItemMessenger":
                    {
                        // Aktualizujem chat
                        try
                        {
                            // Jednorazovo spustim nacitanie vsetkych verejnych sprav
                            if (!_firstEntryToMessenger)
                            {
                                // nefunguje korektne, dlhe nacitavanie a prestane fungovat nacitavanie prihlasenych uzivatelov
//                                _server.GetAllPublicMessages();
                                _firstEntryToMessenger = true;
                            }
                            // spusti timer, kde sa kazde 3s spusti metoda, ktora obnovi zoznam online uzivatelov
                            // ZDROJ - https://stackoverflow.com/questions/6169288/execute-specified-function-every-x-seconds
                            var timer1 = new Timer();
                            timer1.Tick += new EventHandler(Timer_RefreshOnlineUser);
                            timer1.Interval = 3000;
                            timer1.Start();
                        }
                        catch (Exception)
                        {
                            AppendLogMessageToChat("System", "Neočakávana chyba na serveri. Správy neboli načítané.");
                        }
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Metoda, ktoru pouziva Timer na pravidelne spustanie aktualizacie prihlasenych pouzivatelov
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_RefreshOnlineUser(object sender, EventArgs e)
        {
            RefreshOnlineChatUsers();
        }

        private void ListViewTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _selectedTask = (QTask) ListViewTasks.SelectedItem;
            var dialog = new TaskViewWindow(_selectedTask);
            dialog.ShowDialog();
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
                    RecreateDbContext();
                    RefreshListUsers(_dbContext.GetUsers().ToList(), true);
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
            RecreateDbContext();
            UserList = _dbContext.ApplyFilterUsers(userName, userRoles);
            RefreshListUsers(UserList, true);
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
                    "Zmazanie projektu", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                    MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    _dbContext.RemoveProject(_selectedProject);
                    RecreateDbContext();
                    RefreshListProjects(_dbContext.GetProjects().ToList(), true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Nastala neočakávaná chyba a projekt nemohol byť vymazaný!");
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

            RecreateDbContext();
            ProjectList =
                _dbContext.ApplyFilterProjects(projectName, productOwnerName, estimatedEndFrom, estimatedEndTo);
            RefreshListProjects(ProjectList, true);
        }

        private void ListViewTasks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedTask = (QTask) ListViewTasks.SelectedItem;
            ButtonEditTask.IsEnabled = true;
            ButtonRemoveTask.IsEnabled = true;
        }

        private void ButtonNewTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TaskEditWindow(_dbContext, this, null);
            dialog.Show();
        }

        private void ButtonEditTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TaskEditWindow(_dbContext, this, _selectedTask);
            dialog.Show();
        }

        private void ButtonRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show($@"Ste si istý, že chcete zmazať úlohu {_selectedTask.Title}?",
                    "Zmazanie úlohy", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                    MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    _dbContext.RemoveTask(_selectedTask);
                    RecreateDbContext();
                    RefreshListTasks(_dbContext.GetTasks().ToList(), true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Nastala neočakávaná chyba a úloha nemohla byť vymazaná!");
                    Console.WriteLine(exception);
                }
            }
        }

        private void ButtonApplyFilterTask_Click(object sender, RoutedEventArgs e)
        {
            var taskName = (TextBoxFilterTaskName.Text.Trim() != "") ? TextBoxFilterTaskName.Text : null;
            QUser taskAssignedUser = null;
            try
            {
                taskAssignedUser = (QUser) ComboBoxFilterTaskAssigned.SelectedItem;
            }
            catch (Exception)
            {
                // ignored
            }
            QProject taskProject = null;
            try
            {
                taskProject = (QProject) ComboBoxFilterTaskProject.SelectedItem;
            }
            catch (Exception)
            {
                // ignored
            }
            var taskEstimatedEndFrom = DatePickerFilterTaskDeadlineFrom.SelectedDate;
            var taskEstimatedEndTo = DatePickerFilterTaskDeadlineTo.SelectedDate;
            bool? taskIsPrivate = null;
            try
            {
                var item = (PrivateItem) ComboBoxFilterTaskPrivate.SelectedItem;
                if (item.GetKey() == 0) taskIsPrivate = false;
                else if (item.GetKey() == 1) taskIsPrivate = true;
            }
            catch (Exception)
            {
                // ignored
            }

            RecreateDbContext();
            TaskList = _dbContext.ApplyFilterTask(taskName, taskAssignedUser, taskProject, taskEstimatedEndFrom,
                taskEstimatedEndTo, taskIsPrivate);
            RefreshListTasks(TaskList, true);
        }

        private void ButtonChatLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxChatUserName.Text.Trim() == "")
            {
                MessageBox.Show("Zadaj meno používateľa!");
            }
            else
            {
                try
                {
                    // Bol uspesne prihlaseny?
                    if (_server.Login(TextBoxChatUserName.Text))
                    {
                        // Nastavim kontext noveho uzivatela
                        _loggedUser = TextBoxChatUserName.Text;
                        // znefunkcnim nove prihlasovanie
                        TextBoxChatUserName.IsEnabled = false;
                        ButtonChatLogin.IsEnabled = false;
                        TextBlockChatSignIn.Visibility = Visibility.Visible;
                        // zapnem tlacidlo pre odoslanie spravy a odhlasenie
                        ButtonChatSendMessage.IsEnabled = true;
                        ButtonChatLogout.IsEnabled = true;
                    }
                    // Nebol uspesne prihlaseny, cize uzivatel uz existuje
                    else
                    {
                        MessageBox.Show("Takýto používateľ už existuje!");
                    }
                }
                catch (Exception)
                {
                    AppendLogMessageToChat("System", "Neočakávana chyba na serveri. Neboli ste prihlásený.");
                }
            }
        }

        private void ButtonChatSendMessage_Click(object sender, RoutedEventArgs e)
        {
            var toUser = ComboBoxChatListUsers.Text.Trim() != "Všetci" ? ComboBoxChatListUsers.Text : null;

            var message = new Message
            {
                FromUser = "JA",
                ToUser = toUser,
                Text = TextBoxChatMessage.Text
            };
            AppendMessageToChat(message);
            TextBoxChatMessage.Clear();
            try
            {
                _server.SendMessage(_loggedUser, toUser, message.Text);
            }
            catch (Exception)
            {
                AppendLogMessageToChat("System", "Neočakávana chyba na serveri. Správa nebola odoslaná.");
            }
        }

        public void AppendMessageToChat(Message message)
        {
            ListViewChat.Items.Add(message);
        }

        public void AppendLogMessageToChat(string from, string message)
        {
            ListViewChat.Items.Add(message);
        }

        private void ButtonChatLogout_Click(object sender, RoutedEventArgs e)
        {
            if (!UserLogout()) return;
            TextBoxChatUserName.Clear();
            TextBoxChatUserName.IsEnabled = true;
            ButtonChatLogin.IsEnabled = true;
            ButtonChatLogout.IsEnabled = false;
            TextBlockChatSignIn.Visibility = Visibility.Hidden;
            ButtonChatSendMessage.IsEnabled = false;
        }

        private void MenuItemCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    /// <summary>
    /// Pomocna trieda pre naplnenie stavov privatnych uloh vo filtri.
    /// </summary>
    internal class PrivateItem
    {
        private readonly string _name;
        private readonly int _key;

        public PrivateItem(string name, int key)
        {
            _name = name;
            _key = key;
        }

        public int GetKey()
        {
            return _key;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}