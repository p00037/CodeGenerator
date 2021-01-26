using System.Windows;

namespace CodeGenerator.Views
{
    /// <summary>
    /// Menu.xaml の相互作用ロジック
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void AutoCodeGenerator_Click(object sender, RoutedEventArgs e)
        {
            var w = new AutoCodeGenerator();
            w.Owner = GetWindow(this);
            w.ShowDialog();
        }

        private void WpfFody_Click(object sender, RoutedEventArgs e)
        {
            var w = new WpfFody();
            w.Owner = GetWindow(this);
            w.ShowDialog();
        }
    }
}
