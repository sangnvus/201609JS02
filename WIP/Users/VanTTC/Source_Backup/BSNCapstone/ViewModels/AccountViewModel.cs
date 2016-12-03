using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSNCapstone.Models;

namespace BSNCapstone.ViewModels
{
    public class AccountViewModel
    {
        public LoginViewModel Login { get; set; }
        public RegisterViewModel Register { get; set; }
        public AuthorRegisterViewModel AuthorRegister { get; set; }
        public ForgotPasswordViewModel ForgotPassword { get; set; }
    }
}