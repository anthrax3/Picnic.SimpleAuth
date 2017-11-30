using System.ComponentModel.DataAnnotations;

namespace Picnic.SimpleAuth.Model
{
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ReturnUrl
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the IsLoginFailure
        /// </summary>
        public bool IsLoginFailure { get; set; }
    }
}