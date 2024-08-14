using CleanApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace CleanApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
       
        public ApplicationDbContext()
        {
        }

        // Constructor accepting a connection string for runtime
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        //    optionsBuilder.UseMySQL(connectionString);
        //}

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CleanDB;Trusted_Connection=True;");
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Check if optionsBuilder already has a connection string configured
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        // Use the SQL Server connection string from web.config
        //        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        //        optionsBuilder.UseSqlServer(connectionString);
        //    }
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Check if optionsBuilder already has a c onnection string configured
            if (!optionsBuilder.IsConfigured)
            {
                // Use the SQL Server connection string from web.config
                string connectionString = ConfigurationManager.ConnectionStrings["connectionString"]?.ConnectionString;

                // If connectionString is null, provide a default connection string as a fallback
                if (connectionString == null)
                {
                    //connectionString = "Server=DESKTOP-UOQM43T;Database=CleanAppDB;Trusted_Connection=True;TrustServerCertificate=True"; //sql
                    connectionString = "Server=127.0.0.1;Port=3306;Database=CleanAppDB;Uid=root;";
                }

                optionsBuilder.UseMySQL(connectionString);
                
            }
        }


    }
}
