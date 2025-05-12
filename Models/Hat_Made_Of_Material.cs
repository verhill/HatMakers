using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Hat_Made_Of_Material
    {
        public int MaterialId { get; set; }
        public int HatId { get; set; }
        public string? Quantity { get; set; }
        

        [ForeignKey(nameof(HatId))]
        public virtual Hat Hat { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public virtual Material Material { get; set; }

       public Hat_Made_Of_Material() { }
    }
}
