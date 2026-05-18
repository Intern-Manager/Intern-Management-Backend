using InternManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Auth
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Recruitment
    public DbSet<InternProfile> InternProfiles => Set<InternProfile>();
    public DbSet<InternshipCampaign> InternshipCampaigns => Set<InternshipCampaign>();
    public DbSet<CampaignApplication> Applications => Set<CampaignApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();

    // Training
    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<LearningResource> LearningResources => Set<LearningResource>();

    // Internship Tracking
    public DbSet<Mentorship> Mentorships => Set<Mentorship>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskSubmission> TaskSubmissions => Set<TaskSubmission>();
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();

    // Evaluation
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Feedback> Feedback => Set<Feedback>();

    // Communication
    public DbSet<Communication> Communications => Set<Communication>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // Admin
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(r => r.RoleId);
            e.Property(r => r.RoleName).HasMaxLength(50).IsRequired();
            e.Property(r => r.Description).HasMaxLength(256);
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.UserId);
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.AvatarUrl).HasColumnType("nvarchar(max)");
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Status).HasMaxLength(50);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(r => r.Id);
            e.Property(r => r.Token).HasMaxLength(256).IsRequired();
        });

        // InternProfile
        modelBuilder.Entity<InternProfile>(e =>
        {
            e.ToTable("InternProfiles");
            e.HasKey(p => p.InternId);
            e.Property(p => p.Address).HasMaxLength(256);
            e.Property(p => p.University).HasMaxLength(128);
            e.Property(p => p.Major).HasMaxLength(128);
            e.Property(p => p.CvUrl).HasMaxLength(1024);
            e.Property(p => p.LinkedinUrl).HasMaxLength(256);
            e.Property(p => p.GithubUrl).HasMaxLength(256);
        });

        // InternshipCampaign
        modelBuilder.Entity<InternshipCampaign>(e =>
        {
            e.ToTable("InternshipCampaigns");
            e.HasKey(c => c.CampaignId);
            e.Property(c => c.Title).HasMaxLength(256).IsRequired();
            e.Property(c => c.Description).HasMaxLength(2048);
            e.Property(c => c.Requirements).HasMaxLength(2048);
            e.Property(c => c.Department).HasMaxLength(100);
            e.Property(c => c.Location).HasMaxLength(256);
            e.Property(c => c.Status).HasMaxLength(50);
        });

        // CampaignApplication
        modelBuilder.Entity<CampaignApplication>(e =>
        {
            e.ToTable("CampaignApplications");
            e.HasKey(a => a.ApplicationId);
            e.Property(a => a.ApplicantEmail).HasMaxLength(256).IsRequired();
            e.Property(a => a.ApplicantName).HasMaxLength(100).IsRequired();
            e.Property(a => a.ApplicantPhone).HasMaxLength(20);
            e.Property(a => a.CvUrl).HasMaxLength(1024);
            e.Property(a => a.CoverLetter).HasMaxLength(2048);
            e.Property(a => a.Status).HasMaxLength(50);
            e.Property(a => a.Notes).HasMaxLength(1024);
        });

        // Interview
        modelBuilder.Entity<Interview>(e =>
        {
            e.ToTable("Interviews");
            e.HasKey(i => i.InterviewId);
            e.Property(i => i.InterviewType).HasMaxLength(50);
            e.Property(i => i.Location).HasMaxLength(256);
            e.Property(i => i.Status).HasMaxLength(50);
            e.Property(i => i.Feedback).HasMaxLength(2048);
        });

        // TrainingProgram
        modelBuilder.Entity<TrainingProgram>(e =>
        {
            e.ToTable("TrainingPrograms");
            e.HasKey(p => p.ProgramId);
            e.Property(p => p.ProgramName).HasMaxLength(256).IsRequired();
            e.Property(p => p.Description).HasMaxLength(2048);
            e.Property(p => p.Objectives).HasMaxLength(2048);
            e.Property(p => p.Status).HasMaxLength(50);
        });

        // LearningResource
        modelBuilder.Entity<LearningResource>(e =>
        {
            e.ToTable("LearningResources");
            e.HasKey(r => r.ResourceId);
            e.Property(r => r.Title).HasMaxLength(256).IsRequired();
            e.Property(r => r.Description).HasMaxLength(2048);
            e.Property(r => r.ResourceType).HasMaxLength(50);
            e.Property(r => r.FileSizeMb).HasPrecision(10, 2);
        });

        // Mentorship
        modelBuilder.Entity<Mentorship>(e =>
        {
            e.ToTable("Mentorships");
            e.HasKey(m => m.MentorshipId);
            e.Property(m => m.Status).HasMaxLength(50);
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("TaskItems");
            e.HasKey(t => t.TaskId);
            e.Property(t => t.Title).HasMaxLength(256).IsRequired();
            e.Property(t => t.Description).HasMaxLength(2048);
            e.Property(t => t.Priority).HasMaxLength(50);
            e.Property(t => t.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TaskSubmission>(e =>
        {
            e.ToTable("TaskSubmissions");
            e.HasKey(s => s.SubmissionId);
            e.Property(s => s.Status).HasMaxLength(50);
            e.Property(s => s.SubmissionText).HasMaxLength(2048);
            e.Property(s => s.Comments).HasMaxLength(1024);
            e.Property(s => s.Feedback).HasMaxLength(1024);
        });

        // DailyLog
        modelBuilder.Entity<DailyLog>(e =>
        {
            e.ToTable("DailyLogs");
            e.HasKey(l => l.LogId);
            e.Property(l => l.ActivityDescription).HasMaxLength(2048);
            e.Property(l => l.ChallengesFaced).HasMaxLength(1024);
            e.Property(l => l.MentorFeedback).HasMaxLength(1024);
            e.Property(l => l.HoursWorked).HasPrecision(5, 2);
            e.Property(l => l.KpiScore).HasPrecision(5, 2);
        });

        // Assessment
        modelBuilder.Entity<Assessment>(e =>
        {
            e.ToTable("Assessments");
            e.HasKey(a => a.AssessmentId);
            e.Property(a => a.AssessmentType).HasMaxLength(50);
            e.Property(a => a.Strengths).HasMaxLength(1024);
            e.Property(a => a.AreasForImprovement).HasMaxLength(1024);
            e.Property(a => a.Comments).HasMaxLength(2048);
            e.Property(a => a.CommunicationScore).HasPrecision(5, 2);
            e.Property(a => a.OverallRating).HasPrecision(5, 2);
            e.Property(a => a.SoftSkillsScore).HasPrecision(5, 2);
            e.Property(a => a.TeamworkScore).HasPrecision(5, 2);
            e.Property(a => a.TechnicalSkillsScore).HasPrecision(5, 2);
        });

        // Feedback
        modelBuilder.Entity<Feedback>(e =>
        {
            e.ToTable("Feedback");
            e.HasKey(f => f.FeedbackId);
            e.Property(f => f.FeedbackType).HasMaxLength(50);
            e.Property(f => f.Comments).HasMaxLength(2048);
        });

        // Communication
        modelBuilder.Entity<Communication>(e =>
        {
            e.ToTable("Communications");
            e.HasKey(c => c.MessageId);
            e.Property(c => c.Subject).HasMaxLength(256);
            e.Property(c => c.MessageContent).HasMaxLength(4096);
        });

        // Notification
        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(n => n.NotificationId);
            e.Property(n => n.NotificationType).HasMaxLength(50);
            e.Property(n => n.Category).HasMaxLength(50);
            e.Property(n => n.Subject).HasMaxLength(256);
            e.Property(n => n.Content).HasMaxLength(1024);
            e.Property(n => n.RelatedType).HasMaxLength(50);
        });

        // Report
        modelBuilder.Entity<Report>(e =>
        {
            e.ToTable("Reports");
            e.HasKey(r => r.ReportId);
            e.Property(r => r.ReportName).HasMaxLength(256).IsRequired();
            e.Property(r => r.ReportType).HasMaxLength(50);
            e.Property(r => r.Description).HasMaxLength(1024);
            e.Property(r => r.FileFormat).HasMaxLength(20);
        });

        // Attendance
        modelBuilder.Entity<Attendance>(e =>
        {
            e.ToTable("Attendance");
            e.HasKey(a => a.AttendanceId);
            e.Property(a => a.Status).HasMaxLength(50);
            e.Property(a => a.Notes).HasMaxLength(512);
        });

        // Certificate
        modelBuilder.Entity<Certificate>(e =>
        {
            e.ToTable("Certificates");
            e.HasKey(c => c.CertificateId);
            e.Property(c => c.CertificateName).HasMaxLength(256).IsRequired();
            e.Property(c => c.Description).HasMaxLength(1024);
        });

        // Department
        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("Departments");
            e.HasKey(d => d.DepartmentId);
            e.Property(d => d.DepartmentName).HasMaxLength(100).IsRequired();
            e.Property(d => d.Description).HasMaxLength(512);
            e.Property(d => d.Status).HasMaxLength(50);
        });
    }
}
