using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace component.logger.data.log.Model
{
    /// <summary>
    /// LogConfiguration
    /// </summary>
    [Table("LogConfiguration")]
    public class LogConfiguration
    {
        /// <summary>
        /// Gets or sets the log configuration identifier.
        /// </summary>
        /// <value>
        /// The log configuration identifier.
        /// </value>
        [Key]
        [Column("LogConfigId")]
        public Guid LogConfigId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        [StringLength(50)]
        [Column("Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is database.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is database; otherwise, <c>false</c>.
        /// </value>
        [Column("IsDb")]
        public bool IsDb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is oms.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is oms; otherwise, <c>false</c>.
        /// </value>
        [Column("IsOms")]
        public bool IsOms { get; set; }

        //[Column("lc_file")]
        //public bool IsFile { get; set; }

        //[Column("lc_ev")]
        //public bool IsEventViewer { get; set; }

        //[Column("lc_mail")]
        //public bool IsMail { get; set; }

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
    }
}
