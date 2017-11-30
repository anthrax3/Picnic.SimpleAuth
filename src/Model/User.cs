using System;
using Picnic.Model;

namespace Picnic.SimpleAuth.Model
{
    public class User : IPicnicEntity
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the item
        /// </summary>
        public string LastModifyUser { get; set; }

        /// <summary>
        /// Gets or sets the date the item was last modified
        /// </summary>
        public DateTime LastModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        public string Password { get; set; }        
    }
}