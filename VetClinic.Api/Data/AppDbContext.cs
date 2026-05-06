using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Models;

namespace VetClinic.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ClinicService> ClinicServices => Set<ClinicService>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Vaccination> Vaccinations => Set<Vaccination>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Pet>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Pets)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Pet)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Owner)
            .WithMany(u => u.OwnerAppointments)
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian)
            .WithMany(u => u.VeterinarianAppointments)
            .HasForeignKey(a => a.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Service)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MedicalRecord>()
            .HasOne(m => m.Appointment)
            .WithOne(a => a.MedicalRecord)
            .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MedicalRecord>()
            .HasOne(m => m.Pet)
            .WithMany(p => p.MedicalRecords)
            .HasForeignKey(m => m.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MedicalRecord>()
            .HasOne(m => m.Veterinarian)
            .WithMany(u => u.MedicalRecords)
            .HasForeignKey(m => m.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Vaccination>()
            .HasOne(v => v.Pet)
            .WithMany(p => p.Vaccinations)
            .HasForeignKey(v => v.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Vaccination>()
            .HasOne(v => v.Veterinarian)
            .WithMany(u => u.Vaccinations)
            .HasForeignKey(v => v.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Vaccination>()
            .HasOne(v => v.VaccineInventoryItem)
            .WithMany(i => i.Vaccinations)
            .HasForeignKey(v => v.VaccineInventoryItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryTransaction>()
            .HasOne(t => t.InventoryItem)
            .WithMany(i => i.Transactions)
            .HasForeignKey(t => t.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryTransaction>()
            .HasOne(t => t.RelatedMedicalRecord)
            .WithMany(m => m.InventoryTransactions)
            .HasForeignKey(t => t.RelatedMedicalRecordId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<InventoryTransaction>()
            .HasOne(t => t.CreatedByUser)
            .WithMany(u => u.InventoryTransactions)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Pet>()
            .Property(p => p.Gender)
            .HasConversion<string>();

        builder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        builder.Entity<Vaccination>()
            .Property(v => v.Status)
            .HasConversion<string>();

        builder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();

        builder.Entity<Pet>().Property(p => p.Weight).HasPrecision(10, 2);
        builder.Entity<ClinicService>().Property(s => s.Price).HasPrecision(10, 2);
        builder.Entity<MedicalRecord>().Property(m => m.Temperature).HasPrecision(5, 2);
        builder.Entity<MedicalRecord>().Property(m => m.Weight).HasPrecision(10, 2);
        builder.Entity<InventoryItem>().Property(i => i.Quantity).HasPrecision(10, 2);
        builder.Entity<InventoryItem>().Property(i => i.MinQuantity).HasPrecision(10, 2);
        builder.Entity<InventoryItem>().Property(i => i.Price).HasPrecision(10, 2);
        builder.Entity<InventoryTransaction>().Property(t => t.Quantity).HasPrecision(10, 2);
    }
}
