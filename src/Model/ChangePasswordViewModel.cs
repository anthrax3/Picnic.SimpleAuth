using System.ComponentModel.DataAnnotations;

namespace Picnic.SimpleAuth.Model
{
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        [Required(ErrorMessage = "required")]
        public string Current { get; set; }

        /// <summary>
        /// Gets or sets the New
        /// </summary>
        [Required(ErrorMessage = "required")]
        public string New { get; set; }

        /// <summary>
        /// Gets or sets the Confirm
        /// </summary>
        [Required(ErrorMessage = "required")]
        [Compare(nameof(New), ErrorMessage = "doesn't match")]
        public string Confirm { get; set; }
    }
}