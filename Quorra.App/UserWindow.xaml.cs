using System;
using System.Linq;
using System.Windows;
using Quorra.Library;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private QuorraContext _dbContext;
        private readonly MainWindow _mainWindow;
        private readonly QUser _user;
        private Array DataUserRoles { get; set; }

        public UserWindow(QuorraContext dbContext, MainWindow mainWindow, QUser user)
        {
            InitializeComponent();

            _dbContext = dbContext;
            _mainWindow = mainWindow;
            _user = user;

            // Naplnim si hodnoty pouzivatelskych roli
            DataUserRoles = Enum.GetValues(typeof(QUserRole));
            ComboBoxUserRole.ItemsSource = DataUserRoles;

            // Ak user nie je null, jedna sa editaciu a predvyplnim polia
            if (user != null)
            {
                TextBoxUserName.Text = user.Name;
                ComboBoxUserRole.SelectedItem = user.UserRole;
            }
        }

        /// <summary>
        /// Obsluha zatvorenia okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUserCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Obsluha potvrdit zmeny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUserOk_Click(object sender, RoutedEventArgs e)
        {
            var userName = TextBoxUserName.Text.Trim();
            var userRole = ComboBoxUserRole.SelectedItem;

            if (userName == "")
            {
                MessageBox.Show("Používateľské meno je povinné!");
            }
            else
            {
                // Pokial nemam nastaveneho usera, idem vytvorit noveho
                if (_user == null)
                {
                    QUser user = new QUser();
                    user.Name = userName;
                    user.UserRole = (QUserRole) userRole;

                    _dbContext.InsertUser(user);
                }
                // Ak mam nastaveneho usera, idem editovat
                else
                {
                    _user.Name = userName;
                    _user.UserRole = (QUserRole)userRole;

                    _dbContext.UpdateUser(_user);
                }
                RecreateDbContext();
                _mainWindow.RefreshListUsers(_dbContext.GetUsers().ToList(), false);
                Close();
            }
        }

        /// <summary>
        /// Metoda mi znovu vytvori kontext do databazy. Potrebujem to vtedy, kedy aktualizujem data a 
        /// potrebujem ich mat aktualizovane aj na ostatnych klientoch
        /// </summary>
        private void RecreateDbContext()
        {
            _dbContext = new QuorraContext();
        }
    }
}