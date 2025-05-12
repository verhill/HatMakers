using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Order_Contains_Hat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int HatId { get; set; }
        public int OrderId { get; set; }
        public double? HeadSize { get; set; }
        public double? Height { get; set; }

        [ForeignKey(nameof(HatId))]
        public virtual Hat Hat { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        public Order_Contains_Hat() { }
    }
}
