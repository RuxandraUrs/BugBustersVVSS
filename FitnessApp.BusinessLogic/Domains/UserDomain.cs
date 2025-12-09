using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.DataAccess.Interfaces;

namespace FitnessApp.BusinessLoic.Domains
{
    public class UserDomain
    {
        private IUserRepository userRepository { get; set; }

        public UserDomain(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
    }
}
