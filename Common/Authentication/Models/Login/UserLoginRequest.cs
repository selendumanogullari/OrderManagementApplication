﻿using System.ComponentModel.DataAnnotations;

namespace Authentication.Models.Login
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
