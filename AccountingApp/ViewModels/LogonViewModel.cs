using System.ComponentModel.DataAnnotations;

namespace AccountingApp.ViewModels
{
    public class LogonViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}