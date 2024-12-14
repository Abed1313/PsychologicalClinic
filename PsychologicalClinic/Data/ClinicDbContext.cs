using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Models;

namespace PsychologicalClinic.Data
{
    public class ClinicDbContext : IdentityDbContext<Characters>
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<PatientComment> PatientComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Characters to Doctor (One-to-One with Restrict Delete)
            modelBuilder.Entity<Characters>()
                .HasOne(c => c.Doctor)
                .WithOne(d => d.User) // Navigation from Doctor to Characters
                .HasForeignKey<Doctor>(d => d.CharactersId) // Foreign key on Doctor
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Characters to Patient (One-to-One with Restrict Delete)
            modelBuilder.Entity<Characters>()
                .HasOne(c => c.Patient)
                .WithOne(p => p.User) // Navigation from Patient to Characters
                .HasForeignKey<Patient>(p => p.CharactersId) // Foreign key on Patient
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Doctor to Video (One-to-Many)
            modelBuilder.Entity<Video>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Videos)
                .HasForeignKey(v => v.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor to PatientComment (One-to-Many)
            modelBuilder.Entity<PatientComment>()
                .HasOne(pc => pc.Doctor)
                .WithMany(d => d.Comments)
                .HasForeignKey(pc => pc.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient to PatientComment (One-to-Many)
            modelBuilder.Entity<PatientComment>()
                .HasOne(pc => pc.Patient)
                .WithMany(p => p.Comments)
                .HasForeignKey(pc => pc.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Disease to Patient (Many-to-Many)
            modelBuilder.Entity<Disease>()
                .HasMany(d => d.Patients)
                .WithMany(p => p.DiseaseHistory)
                .UsingEntity<Dictionary<string, object>>(
                    "PatientDisease", // Join table name
                    j => j.HasOne<Patient>().WithMany().HasForeignKey("PatientId"),
                    j => j.HasOne<Disease>().WithMany().HasForeignKey("DiseaseId"));

            // Configure additional properties if necessary

            modelBuilder.Entity<Video>()
                .Property(v => v.Type)
                .HasConversion<string>();

            seedRoles(modelBuilder, "Doctor", "update", "read", "delete", "create");
            seedRoles(modelBuilder, "Patient", "read");
        }
        private void seedRoles(ModelBuilder modelBuilder, string roleName, params string[] permission)
        {
            var role = new IdentityRole
            {
                Id = roleName.ToLower(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                ConcurrencyStamp = Guid.Empty.ToString()
            };
            // add claims for the users
            var claims = permission.Select(permission => new IdentityRoleClaim<string>
            {
                Id = Guid.NewGuid().GetHashCode(),
                // Unique identifier
                RoleId = role.Id,
                ClaimType = "permission",
                ClaimValue = permission
            });
            // Seed the role and its claims
            modelBuilder.Entity<IdentityRole>().HasData(role);
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(claims);
        }
    }
    
}
