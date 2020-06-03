using component.logger.data.log.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace component.logger.data.log.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DbContext" />
    public class LogDataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogDataContext"/> class.
        /// </summary>
        public LogDataContext()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDataContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public LogDataContext(DbContextOptions<LogDataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        public DbSet<Component> Components { get; set; }

        /// <summary>
        /// Gets or sets the component configurations.
        /// </summary>
        /// <value>
        /// The component configurations.
        /// </value>
        public DbSet<ComponentConfiguration> ComponentConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the error logs.
        /// </summary>
        /// <value>
        /// The error logs.
        /// </value>
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        /// <summary>
        /// Gets or sets the log configurations.
        /// </summary>
        /// <value>
        /// The log configurations.
        /// </value>
        public DbSet<LogConfiguration> LogConfigurations { get; set; }

        //public DbSet<UICulture> UICultures { get; set; }
        //public DbSet<UIMessage> UIMessages { get; set; }
        //public DbSet<UIMessageText> UIMessageTexts { get; set; }

        /// <summary>
        /// Gets or sets the application setting.
        /// </summary>
        /// <value>
        /// The application setting.
        /// </value>
        public DbSet<AppSetting> AppSetting { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Component>()
        //        .Property(e => e.Name)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Component>()
        //        .HasMany(e => e.ComponentConfigurations)
        //        .WithOne(e => e.Component)
        //        .HasForeignKey(e => e.ComponentId)
        //        .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);

        //    modelBuilder.Entity<ComponentConfiguration>()
        //        .Property(e => e.KeyName)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.Severity)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.Logger)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.File)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.Method)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.Message)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.MessageData)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.StackTrace)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ErrorLog>()
        //        .Property(e => e.Exception)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<LogConfiguration>()
        //        .Property(e => e.Type)
        //        .IsUnicode(false);
        //}

        /// <summary>
        /// Called when [configuring].
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppJsonConfig.Configuration.GetConnectionString("SolutionLoggerDataConnection"));
            base.OnConfiguring(optionsBuilder);
        }
    }
}