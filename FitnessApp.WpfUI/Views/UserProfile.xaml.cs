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
using FitnessApp.Models;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.DataAccess.Interfaces;
using FitnessApp.DataAccess.Repositories;
using FitnessApp.WpfUI.ViewModels;
using DbConnection = FitnessApp.DataAccess.DbConnection;

namespace FitnessApp.WpfUI.Views
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Window
    {
        public UserProfile(User user)
        {
            InitializeComponent();

            string connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connString))
                throw new InvalidOperationException("DB_CONNECTION_STRING env variable not found.");

            DbConnection dbConnection = new DbConnection(connString);
            IUserRepository userRepository = new UserRepository(dbConnection);
            IUserDomain userDomain = new UserDomain(userRepository);

            DataContext = new UserProfileVM(user, userDomain);
        }
    }
}
