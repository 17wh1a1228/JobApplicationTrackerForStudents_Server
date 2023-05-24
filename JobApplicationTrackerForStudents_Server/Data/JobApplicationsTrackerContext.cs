using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using JobApplicationTrackerForStudents_Server.Models;
using JobApplicationTrackerForStudents_Server.Dtos;
using System.Reflection;


namespace JobApplicationTrackerForStudents_Server.Data
{
    public partial class JobApplicationsTrackerContext : IdentityDbContext<ApplicationsUser>
    {
        public JobApplicationsTrackerContext()
        {
        }

        public JobApplicationsTrackerContext(DbContextOptions<JobApplicationsTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Application> Applications { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        //public virtual DbSet<SignUp> Registrations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.JobId).IsFixedLength();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                //entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.Username)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
