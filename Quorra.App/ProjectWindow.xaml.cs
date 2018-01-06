using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        private readonly QuorraContext _dbContext;
        private readonly MainWindow _mainWindow;
        private readonly QProject _project;

        public readonly List<QUser> UsersList;

        public ProjectWindow(QProject project, MainWindow mainWindow, QuorraContext dbContext)
        {
            InitializeComponent();

            _project = project;
            _mainWindow = mainWindow;
            _dbContext = dbContext;

            // Pociatocne naplnenie comboboxu uzivatelov
            UsersList = _dbContext.GetUsers().ToList();
            ComboBoxProductOwner.SelectedValuePath = "Id";
            ComboBoxProductOwner.DisplayMemberPath = "Name";
            ComboBoxProductOwner.ItemsSource = UsersList;

            // Ak je naplnenu project, ide o editaciu
            if (project != null)
            {
                TextBoxProjectName.Text = project.Name;
                ComboBoxProductOwner.SelectedItem = project.ProductOwner;
                DatePickerEstimatedEnd.SelectedDate = project.EstimatedEnd;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            var name = TextBoxProjectName.Text.Trim();
            var productOwner = (QUser) ComboBoxProductOwner.SelectedItem;
            DateTime? estimatedEnd = DatePickerEstimatedEnd.SelectedDate;

            if (name == "")
            {
                MessageBox.Show("Názov projektu je povinné!");
            }
            else
            {
                // Pokial nemam nastaveny projekt, idem vytvorit novy
                if (_project == null)
                {
                    QProject project = new QProject
                    {
                        Name = name,
                        ProductOwnerId = productOwner?.Id ?? null, // Ked nevyberiem nikoho, nastavi sa null
                        EstimatedEnd = estimatedEnd
                    };

                    _dbContext.InsertProject(project);
                }
                // Ak mam nastaveny projekt, idem editovat
                else
                {
                    _project.Name = name;
                    _project.ProductOwnerId = productOwner?.Id ?? null;
                    _project.EstimatedEnd = estimatedEnd;

                    _dbContext.UpdateProject(_project);
                }
                _mainWindow.RefreshListProjects(_dbContext.GetProjects().ToList(), false);
                Close();
            }
        }
    }
}