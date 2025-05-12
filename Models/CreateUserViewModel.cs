using System.ComponentModel.DataAnnotations;

namespace hatmaker_team2.Models
{
    public class CreateUserViewModel
    {
            [Required]
            [Display(Name = "Förnamn")]
            [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s]+$", ErrorMessage = "Förnamnet får endast innehålla bokstäver.")]

            public string Firstname { get; set; }

            [Required]
            [Display(Name = "Efternamn")]
            [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s]+$", ErrorMessage = "Förnamnet får endast innehålla bokstäver.")]

            public string Lastname { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            [Display(Name = "Är Admin?")]
            public bool IsAdmin { get; set; }
    }
}

