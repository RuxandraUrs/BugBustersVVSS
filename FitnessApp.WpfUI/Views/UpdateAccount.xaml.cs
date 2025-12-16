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
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.Models;
using FitnessApp.WpfUI.ViewModels;

namespace FitnessApp.WpfUI.Views
{
    /// <summary>
    /// Interaction logic for UpdateAccount.xaml
    /// </summary>
    public partial class UpdateAccount : Window
    {
        public UpdateAccount(User user, IUserDomain userDomain)
        {
            InitializeComponent();
            DataContext = new UpdateAccountVM(user, userDomain);
        }
    }
}
