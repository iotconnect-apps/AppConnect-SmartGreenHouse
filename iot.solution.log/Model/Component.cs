using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace component.logger.data.log.Model
{
    /// <summary>
    /// Component
    /// </summary>
    [Table("Component")]
    public class Component
    {
        /// <summary>
        /// Gets or sets the component identifier.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        [Key]
        [Column("ComponentId")]
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [Column("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        [Column("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        /// <value>
        /// The name of the key.
        /// </value>
        [Required]
        [StringLength(100)]
        [Column("KeyName")]
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets the component configurations.
        /// </summary>
        /// <value>
        /// The component configurations.
        /// </value>
        public virtual ICollection<ComponentConfiguration> ComponentConfigurations { get; set; }
    }
}
