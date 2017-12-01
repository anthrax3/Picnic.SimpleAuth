using System.ComponentModel.DataAnnotations;

namespace Picnic.SimpleAuth.Model
{
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [Required(ErrorMessage = "required")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [Required(ErrorMessage = "required")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ReturnUrl
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}