using System.ComponentModel.DataAnnotations;

namespace hatmaker_team2.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vänligen skriv en epost.")]
        [StringLength(255)]
        public string Epost { get; set; }

        [Required(ErrorMessage = "Vänligen skriv lösenord.")]
        [DataType(DataType.Password)]
        public string Losenord { get; set; }
        public bool RememberMe { get; set; }
    }
}
