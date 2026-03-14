using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public Address? Address { get; set; }
    }
}
