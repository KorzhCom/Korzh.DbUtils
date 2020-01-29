using Microsoft.EntityFrameworkCore;

using DbUtilsDemo.Models;

namespace DbUtilsDemo
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected AppDbContext(DbContextOptions options) 
            : base(options)
        { }

        #region NWind
        public DbSet<Category> Categories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Shipper> Shippers { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .ToTable("Categories")
                .Property(c => c.Id).HasColumnName("CategoryID");

            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customer>()
                .ToTable("Customers")
                .Property(c => c.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Customer>()
                .Property(c => c.Id).HasColumnName("CustomerID");

            modelBuilder.Entity<Customer>()
                 .HasKey(c => c.Id);

            modelBuilder.Entity<Employee>()
                .ToTable("Employees")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Id).HasColumnName("EmployeeID");

            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.Id);

            modelBuilder.Entity<Product>()
                .Property(p => p.Id).HasColumnName("ProductID");

            modelBuilder.Entity<Order>()
               .ToTable("Orders")
               .Property(o => o.RowVersion)
               .IsRowVersion();

            modelBuilder.Entity<Order>()
               .Property(o => o.Id).HasColumnName("OrderID");

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<OrderDetail>()
                .ToTable("Order_Details")
                .HasKey(od => new { od.OrderID, od.ProductID });

            modelBuilder.Entity<Shipper>()
                .ToTable("Shippers")
                .HasKey(s => s.Id);

            modelBuilder.Entity<Shipper>()
              .Property(s => s.Id).HasColumnName("ShipperID");

            modelBuilder.Entity<Supplier>()
                .ToTable("Suppliers")
                .HasKey(s => s.Id);

            modelBuilder.Entity<Shipper>()
                .Property(s => s.Id).HasColumnName("SupplierID");
        }
    }
}