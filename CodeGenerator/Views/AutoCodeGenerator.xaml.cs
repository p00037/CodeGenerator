using System.Windows;
using CodeGenerator.ViewModel;

namespace CodeGenerator.Views
{
    /// <summary>
    /// AutoCodeGenerator.xaml の相互作用ロジック
    /// </summary>
    public partial class AutoCodeGenerator : Window
    {
        AutoCodeGeneratorViewModel autoCodeGeneratorViewModel;

        public AutoCodeGenerator()
        {
            InitializeComponent();

            DataContext = this.autoCodeGeneratorViewModel = new AutoCodeGeneratorViewModel(this);
        }
    }
}
