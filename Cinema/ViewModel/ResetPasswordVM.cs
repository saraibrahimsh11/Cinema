using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModel
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
