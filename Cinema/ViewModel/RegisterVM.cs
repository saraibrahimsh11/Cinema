using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModel
{
    public class RegisterVM
    {
        public string Name { get; set; } 
        public string UserName { get; set; }
        [EmailAddress , DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password) , Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } 
  
    }
}
