using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace component.logger.data.log.Model
{
    /// <summary>
    /// ComponentConfiguration
    /// </summary>
    [Table("ComponentConfiguration")]
    public class ComponentConfiguration
    {
        /// <summary>
        /// Gets or sets the component configuration identifier.
        /// </summary>
        /// <value>
        /// The component configuration identifier.
        /// </value>
        [Key]
        [Column("ComponentConfigId")]
        public Guid ComponentConfigId { get; set; }

        /// <summary>
        /// Gets or sets the component identifier.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        [Column("ComponentId")]
        public Guid? ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [Column("applicationCode")]
        public string ApplicationCode { get; set; }

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
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug; otherwise, <c>false</c>.
        /// </value>
        [Column("Debug")]
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is information.
        /// </summary>
        /// <value>
        ///   <c>true</c> if information; otherwise, <c>false</c>.
        /// </value>
        [Column("Info")]
        public bool Info { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is warn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if warn; otherwise, <c>false</c>.
        /// </value>
        [Column("Warn")]
        public bool Warn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if error; otherwise, <c>false</c>.
        /// </value>
        [Column("Error")]
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is fatal.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fatal; otherwise, <c>false</c>.
        /// </value>
        [Column("Fatal")]
        public bool Fatal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is subscribe error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if subscribe error; otherwise, <c>false</c>.
        /// </value>
        [Column("SubscribeError")]
        public bool SubscribeError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is subscribe debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if subscribe debug; otherwise, <c>false</c>.
        /// </value>
        [Column("SubscribeDebug")]
        public bool SubscribeDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is cron job debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cron job debug; otherwise, <c>false</c>.
        /// </value>
        [Column("CronJobDebug")]
        [DefaultValue(0)]
        public bool? CronJobDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is cron job info.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cron job info; otherwise, <c>false</c>.
        /// </value>
        [Column("CronJobInfo")]
        [DefaultValue(0)]
        public bool? CronJobInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is cron job warn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cron job warn; otherwise, <c>false</c>.
        /// </value>
        [Column("CronJobWarn")]
        [DefaultValue(0)]
        public bool? CronJobWarn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is cron job error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cron job error; otherwise, <c>false</c>.
        /// </value>
        [Column("CronJobError")]
        [DefaultValue(0)]
        public bool? CronJobError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentConfiguration"/> is cron job fatal.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cron job fatal; otherwise, <c>false</c>.
        /// </value>
        [Column("CronJobFatal")]
        [DefaultValue(0)]
        public bool? CronJobFatal { get; set; }
       
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        [Column("CreatedBy")]
        public Guid? CreatedBy { get; set; }

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
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        [Column("ModifiedBy")]
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is oms log.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is oms log; otherwise, <c>false</c>.
        /// </value>
        [Column("IsOmsLog")]
        public bool IsOmsLog { get; set; }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public virtual Component Component { get; set; }
    }
}
