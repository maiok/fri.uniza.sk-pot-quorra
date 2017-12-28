using System.Windows;
using Quorra.Model;

namespace Quorra.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly QuorraContext _dbContext;
        public MainWindow()
        {
            InitializeComponent();

            _dbContext = new QuorraContext();
        }
    }
}
