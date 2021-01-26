using System.Windows;
using System.Windows.Controls;
using CodeGenerator.ViewModel;

namespace CodeGenerator.Views
{
    /// <summary>
    /// WpfFody.xaml の相互作用ロジック
    /// </summary>
    public partial class WpfFody : Window
    {
        AutoCodeGeneratorFodyViewModel viewModel;

        public WpfFody()
        {
            InitializeComponent();
            
            DataContext = this.viewModel = new AutoCodeGeneratorFodyViewModel(this);
        }

    }
}
