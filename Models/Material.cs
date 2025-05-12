using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hatmaker_team2.Models
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MID { get; set; }
        [Required]
        public string Name { get; set; }
        //public decimal? Price { get; set; }

        public virtual ICollection<Hat_Made_Of_Material> MaterialInHat { get; set; } = new List<Hat_Made_Of_Material>();

        public Material() { }
    }
}
