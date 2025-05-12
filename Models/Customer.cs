using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CID { get; set; }

        [Required(ErrorMessage = "Vänligen lämna inte förnamn tomt.")]
        [RegularExpression(@"^[\p{L}\s]+$")]
        [StringLength(50)]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Vänligen lämna inte förnamn tomt.")]
        [RegularExpression(@"^[\p{L}\s]+$")]
        [StringLength(50)]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Vänligen ange en email")]
        [StringLength(40, ErrorMessage = "Max 40 tecken")]
        [RegularExpression("^\\S+@\\S+\\.\\S+$", ErrorMessage = "Vänligen ange en giltig email.")]
        //[RegularExpression(@"\A(?:[a-zA-Z0-9!#$%&'+/=?^_`{|}~-]+(?:.[a-zA-Z0-9!#$%&'+/=?^_`{|}~-]+)@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-][a-zA-Z0-9])?.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)\Z", ErrorMessage = "Ange en giltig emailadress.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vänligen ange ett telefonnummer")]
        [RegularExpression(@"^[\d\s\+\-]{1,15}$", ErrorMessage = "Vänligen ange ett giltigt telefonnummer.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vänligen ange adress.")]
        [RegularExpression(@"^[\p{L}\d\s\-]+$", ErrorMessage = "Vänligen ange en giltig adress.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vänligen ange stad.")]
        [RegularExpression(@"^[\p{L}\d\s\-]+$", ErrorMessage = "Vänligen ange en giltig stad.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Vänligen ange land.")]
        [RegularExpression(@"^[\p{L}\d\s\-]+$", ErrorMessage = "Vänligen ange ett giltigt land.")]
        public string Country { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Order> HasOrders { get; set; } = new List<Order>();

        public Customer() { }
    }
}
