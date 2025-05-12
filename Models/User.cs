using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace hatmaker_team2.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Vänligen lämna inte förnamn tomt.")]
        [RegularExpression(@"^[\p{L}\s]+$")]
        [StringLength(50)]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Vänligen lämna inte efternamn tomt.")]
        [RegularExpression(@"^[\p{L}\s]+$")]
        [StringLength(50)]
        public string Lastname { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsGuest { get; set; } = false;

        public bool IsDeactivated { get; set; } = false;


        public virtual ICollection<User_Manage_Hat_Orders> UserManageHatsInOrder { get; set; } = new List<User_Manage_Hat_Orders>();
        public virtual ICollection<Order> CreatedOrders { get; set; } = new List<Order>();

        public User() { }
    }
}
