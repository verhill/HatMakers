using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Hat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HID { get; set; }
        
        [Required(ErrorMessage = "Vänligen lämna inte namn tomt.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Endast bokstäver och mellanslag tillåtna.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vänligen använd siffror")]
        [Range(0, 100000, ErrorMessage = "Pris måste vara mellan 0 och 100000")]
       

        public double Price { get; set; }
        public string? Picture { get; set; }    
        public bool IsSpecial { get; set; } = false;
        public string? SpecialDescription { get; set; }
        public bool IsDeactivated { get; set; } = false; 

        public virtual ICollection<Order_Contains_Hat> HatInOrder { get; set; } = new List<Order_Contains_Hat>();
        public virtual ICollection<User_Manage_Hat_Orders> HatManagedByUser { get; set; } = new List<User_Manage_Hat_Orders>();
        public virtual ICollection<Hat_Made_Of_Material> MaterialUsedInHat { get; set; } = new List<Hat_Made_Of_Material>();


        public Hat() { }
    }
}
