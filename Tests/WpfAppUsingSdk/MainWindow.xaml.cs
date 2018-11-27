using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PCLCommandBase;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
            CancelCommand command = new CancelCommand(new cmd());
        }

        public class cmd : CommandBase
        {
            protected override Task ExecuteCoreAsync(object parameter, CancellationToken cancellationToken)
            {
                return Task.CompletedTask; 
            }
        }
    }
}
