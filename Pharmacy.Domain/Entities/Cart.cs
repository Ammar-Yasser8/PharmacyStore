using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Entities
{
    public class Cart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
