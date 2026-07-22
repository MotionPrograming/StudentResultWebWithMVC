using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentResultAppWithMVC.Models.Entity;

namespace StudentResultAppWithMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Department> departments { get; set; }
        public DbSet<Student> students { get; set; }
        public DbSet<Result> results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.DepartmentName).IsRequired().HasMaxLength(100);
            });
            builder.Entity<Student>(entity =>
            {
                entity.ToTable("students");
                entity.HasKey(s => s.StudentId);
                entity.Property(s => s.StudentName).IsRequired().HasMaxLength(50);
                entity.Property(s => s.StudentAge);
                entity.Property(s => s.AverageMark).HasColumnType("decimal(5,2)");
                entity.Property(s => s.TotalMark).HasColumnType("decimal(5,2)");
                entity.Property(s => s.CGPA).HasColumnType("decimal(3,2)");
                entity.Property(s => s.LetterGrade).HasMaxLength(5);
                entity.Property(s => s.DepartmentId).IsRequired();

                entity.HasOne(s => s.department).WithMany(s => s.Students).HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.result).WithOne(r => r.student).HasForeignKey<Result>(r => r.StudentId).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
            });
            builder.Entity<Result>(entity => {
                entity.ToTable("results");
                entity.HasKey(r => r.ResultId);
                entity.Property(r => r.PhysicsMark).HasColumnType("decimal(5,2)");
                entity.Property(r => r.ChemistryMark).HasColumnType("decimal(5,2)");
                entity.Property(r => r.MathMark).HasColumnType("decimal(5,2)");
                entity.Property(r => r.StudentId).IsRequired();
            });

        }
    }
}
