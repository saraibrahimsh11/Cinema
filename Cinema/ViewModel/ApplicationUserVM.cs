using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModel
{
    public class ApplicationUserVM
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password) , Compare(nameof(NewPassword))]
        public string? ConfirmNewPassword { get; set; }
    }
}
