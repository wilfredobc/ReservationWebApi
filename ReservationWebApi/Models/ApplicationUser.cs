using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationWebApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(20)]
        public string Gender { get; set; }
    }
}
