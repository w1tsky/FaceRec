using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRec.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string UserData { get; set; }

        public User(string login, string password, UserRole role, string userData)
        {
            Login = login;
            Password = password;
            Role = role;
            UserData = userData;
        }

        public override string ToString()
        {
            return Login;
        }
    }
}

