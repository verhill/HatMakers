using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace hatmaker_team2.Models
{
    public class HatViewModel
    {
        public Hat Hat { get; set; }

        [Display(Name = "Material")]
        public List<int> SelectedMaterialIds { get; set; } = new List<int>();

        public List<SelectListItem> Materials { get; set; } = new List<SelectListItem>();
    }
}
