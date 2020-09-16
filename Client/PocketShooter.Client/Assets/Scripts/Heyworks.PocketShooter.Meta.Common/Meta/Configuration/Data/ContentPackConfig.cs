using Heyworks.PocketShooter.Meta.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class ContentPackConfig
    {
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the content of this pack.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<IContentIdentity> Content { get; set; } = new List<IContentIdentity>();
    }
}
