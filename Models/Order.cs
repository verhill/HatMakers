using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OID { get; set; }

        public DateOnly? DeliveryDate { get; set; }

        [Required]
        public string Status { get; set; } = "Ej behandlad";

        public bool ExpressDelivery { get; set; } = false;

        [Required]
        public double TotPrice { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? CustomsNumbers { get; set; }

        public string CreatedById { get; set; }

        public int CustomerId {  get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<User_Manage_Hat_Orders> UserManageHatsInOrder { get; set; } = new List<User_Manage_Hat_Orders>();
        public virtual ICollection<Order_Contains_Hat> HatsInOrder { get; set; } = new List<Order_Contains_Hat>();

        public Order() { }

    }
}
