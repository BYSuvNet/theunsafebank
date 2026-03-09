using Microsoft.EntityFrameworkCore;
using theunsafebank.Models;

namespace theunsafebank.Data;

public class BankContext : DbContext
{
    public BankContext(DbContextOptions<BankContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<LoginAttempt> LoginAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Account)
            .WithOne(a => a.Customer)
            .HasForeignKey<Account>(a => a.CustomerId);

        modelBuilder.Entity<Transfer>()
            .HasOne(t => t.FromAccount)
            .WithMany(a => a.TransfersFrom)
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transfer>()
            .HasOne(t => t.ToAccount)
            .WithMany(a => a.TransfersTo)
            .HasForeignKey(t => t.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed an admin customer with name admin and password admin.
        // Also add an account for that customer with 100,000SEK.

        var adminCustomer = new Customer
        {
            Id = 1,
            FullName = "Admin",
            Username = "admin",
            Password = "admin", // FIXME: Plain text password! Use Bcrypt to hash passwords in a real application
            CustomerNumber = BankNumberGenerator.GenerateCustomerNumber()
        };

        var theBanksAccount = new Account
        {
            Id = 1,
            AccountNumber = BankNumberGenerator.GenerateAccountNumber(),
            Balance = 100000m,
            CustomerId = adminCustomer.Id
        };

        var firstCustomer = new Customer
        {
            Id = 2,
            FullName = "gustav",
            Username = "gus",
            Password = "gus",
            CustomerNumber = BankNumberGenerator.GenerateCustomerNumber()
        };

        var firstCustomerAccount = new Account
        {
            Id = 2,
            AccountNumber = BankNumberGenerator.GenerateAccountNumber(),
            Balance = 10000m,
            CustomerId = firstCustomer.Id
        };

        var secondCustomer = new Customer
        {
            Id = 3,
            FullName = "kråkan",
            Username = "kråkan",
            Password = "kråkan",
            CustomerNumber = BankNumberGenerator.GenerateCustomerNumber()
        };

        var secondCustomerAccount = new Account
        {
            Id = 3,
            AccountNumber = BankNumberGenerator.GenerateAccountNumber(),
            Balance = 10000m,
            CustomerId = secondCustomer.Id
        };

        modelBuilder.Entity<Customer>().HasData(adminCustomer);
        modelBuilder.Entity<Account>().HasData(theBanksAccount);
        modelBuilder.Entity<Customer>().HasData(firstCustomer);
        modelBuilder.Entity<Account>().HasData(firstCustomerAccount);
        modelBuilder.Entity<Customer>().HasData(secondCustomer);
        modelBuilder.Entity<Account>().HasData(secondCustomerAccount);
    }
}
