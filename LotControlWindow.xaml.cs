using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using Ghost.ViewModels;

namespace Ghost.Views
{
    /// <summary>
    /// LotControlWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LotControlWindow : Window
    {
        LotControlWindowViewModel vm = new LotControlWindowViewModel();


        public LotControlWindow()
        {
            InitializeComponent();
            DataContext = vm;

            if (vm.CloseWindowAction == null)
            {
                vm.CloseWindowAction = new Action(this.Close);
            }
        }
    }
}
