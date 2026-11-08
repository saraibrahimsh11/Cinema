using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModel
{
    public class ResendEmailConfirmationVM
    {
        [Required(ErrorMessage = "Enter your username or email")]
        [Display(Name = "Username or email")]
        public string UserNameOrEmail { get; set; }
    }
}
