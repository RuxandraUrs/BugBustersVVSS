using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.BusinessLogic.Domains;
using FitnessApp.Models;

namespace FitnessApp.WpfUI.ViewModels
{
    public class UserProfileVM : BaseVM
    {
        private readonly IUserDomain _userDomain;
        private User currentUser;

        public UserProfileVM(User user, IUserDomain userDomain)
        {
            _userDomain = userDomain;
            currentUser = user;
        }

        public string Username => currentUser?.Username ?? string.Empty;
        public string WelcomeMessage => $"Welcome, {Username}!";
    }
}
