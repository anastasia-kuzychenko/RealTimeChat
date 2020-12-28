using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Nickname length must be at least 3")]
        [Remote(action: "CheckNickname", controller: "Home", ErrorMessage = "Nickname is already in use")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Invalid symbols")]
        public string Nickname { get; set; }
    }
}
