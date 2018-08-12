using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationWebApi.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        [StringLength(30, ErrorMessage ="El nombre de usuario no puede exceder los 30 caracteres")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El genero no puede exceder los 20 caracteres")]
        public string Gender { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
