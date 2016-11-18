using System.ComponentModel.DataAnnotations;

namespace Vault.WebHost.Models.Account
{
    public class SignUpModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [MaxLength(14)]
        public string Password { get; set; }
    }
}