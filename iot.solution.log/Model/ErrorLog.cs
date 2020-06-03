using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace component.logger.data.log.Model
{
    /// <summary>
    /// ErrorLog
    /// </summary>
    [Table("ErrorLog")]
    public class ErrorLog
    {
        /// <summary>
        /// Gets or sets the log identifier.
        /// </summary>
        /// <value>
        /// The log identifier.
        /// </value>
        [Key]
        [Column("LogId")]
        public Guid LogId { get; set; }

        /// <summary>
        /// Gets or sets the log date.
        /// </summary>
        /// <value>
        /// The log date.
        /// </value>
        [Column("LogDate")]
        public DateTime? LogDate { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [Required]
        [StringLength(30)]
        [Column("Severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [Required]
        [StringLength(100)]
        [Column("Logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [StringLength(200)]
        [Column("LogFile")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [StringLength(150)]
        [Column("Method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [StringLength(100)]
        [Column("Identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [Required]
        [Column("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [Column("MessageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        [Column("StackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [Column("Exception")]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [StringLength(30)]
        [Column("ComponentStatus")]
        public string ComponentStatus { get; set; }

        /// <summary>
        /// Gets or sets the applicationCode code.
        /// </summary>
        /// <value>
        /// The applicationCode code.
        /// </value>
        [StringLength(100)]
        [Column("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [StringLength(100)]
        [Column("ErrorCode")]
        public string ErrorCode { get; set; }
    }
}
