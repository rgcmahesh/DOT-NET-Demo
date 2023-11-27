using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Themis.Model;

namespace Themis.Data
{
    public class ThemisContext : DbContext
    {
        public ThemisContext (DbContextOptions<ThemisContext> options)
            : base(options)
        {
        }

        public DbSet<Themis.Model.User> Users { get; set; } = default!;
        public DbSet<Themis.Model.Course> Courses { get; set; } = default!;
        public DbSet<Themis.Model.Exam> Exams { get; set; } = default!;
        public DbSet<Themis.Model.Question> Questions { get; set; } = default!;

        public DbSet<Themis.Model.ExamResult> ExamResult { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // optionsBuilder.UseSqlServer("your_connection_string");
            // Configure the ExamResult foreign key constraints
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.User)
                .WithMany(u => u.ExamResults)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Restrict); // You can use Restrict, SetNull, SetDefault, etc.

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Restrict); // You can use Restrict, SetNull, SetDefault, etc.

        }

    }
}
