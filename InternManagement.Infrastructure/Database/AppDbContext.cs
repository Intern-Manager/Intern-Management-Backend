using InternManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<InternProfile> InternProfiles => Set<InternProfile>();
    public DbSet<InternshipCampaign> InternshipCampaigns => Set<InternshipCampaign>();
    public DbSet<CampaignApplication> Applications => Set<CampaignApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<LearningResource> LearningResources => Set<LearningResource>();
    public DbSet<Mentorship> Mentorships => Set<Mentorship>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Feedback> Feedback => Set<Feedback>();
    public DbSet<Communication> Communications => Set<Communication>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.RoleName).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.RoleName).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.Property(x => x.AvatarUrl).HasMaxLength(255);
            e.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Inactive");
            e.Property(x => x.EmailVerificationToken).HasMaxLength(255);
            e.Property(x => x.PasswordResetToken).HasMaxLength(255);
            e.Property(x => x.EmailVerified).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InternProfile>(e =>
        {
            e.ToTable("Intern_Profiles");
            e.HasKey(x => x.InternId);
            e.Property(x => x.University).HasMaxLength(255);
            e.Property(x => x.Major).HasMaxLength(100);
            e.Property(x => x.CvUrl).HasMaxLength(255);
            e.Property(x => x.LinkedinUrl).HasMaxLength(255);
            e.Property(x => x.GithubUrl).HasMaxLength(255);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithOne().HasForeignKey<InternProfile>(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InternshipCampaign>(e =>
        {
            e.ToTable("Internship_Campaigns");
            e.HasKey(x => x.CampaignId);
            e.Property(x => x.Title).HasMaxLength(255).IsRequired();
            e.Property(x => x.NumberOfPositions).HasDefaultValue(1);
            e.Property(x => x.Department).HasMaxLength(100);
            e.Property(x => x.Location).HasMaxLength(255);
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Draft");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CampaignApplication>(e =>
        {
            e.ToTable("Applications");
            e.HasKey(x => x.ApplicationId);
            e.Property(x => x.ApplicantEmail).HasMaxLength(100).IsRequired();
            e.Property(x => x.ApplicantName).HasMaxLength(100).IsRequired();
            e.Property(x => x.ApplicantPhone).HasMaxLength(20);
            e.Property(x => x.CvUrl).HasMaxLength(255);
            e.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Pending");
            e.Property(x => x.AppliedDate).HasDefaultValueSql("GETDATE()");
            e.HasOne<InternshipCampaign>().WithMany().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.ReviewedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Interview>(e =>
        {
            e.ToTable("Interviews");
            e.HasKey(x => x.InterviewId);
            e.Property(x => x.DurationMinutes).HasDefaultValue(60);
            e.Property(x => x.InterviewType).HasMaxLength(20).HasDefaultValue("Video");
            e.Property(x => x.MeetingLink).HasMaxLength(255);
            e.Property(x => x.Location).HasMaxLength(255);
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<InternshipCampaign>().WithMany().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<CampaignApplication>().WithMany().HasForeignKey(x => x.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InterviewerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrainingProgram>(e =>
        {
            e.ToTable("Training_Programs");
            e.HasKey(x => x.ProgramId);
            e.Property(x => x.ProgramName).HasMaxLength(255).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Planning");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.CoordinatorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LearningResource>(e =>
        {
            e.ToTable("Learning_Resources");
            e.HasKey(x => x.ResourceId);
            e.Property(x => x.Title).HasMaxLength(255).IsRequired();
            e.Property(x => x.ResourceUrl).HasMaxLength(255);
            e.Property(x => x.ResourceType).HasMaxLength(20).HasDefaultValue("Document");
            e.Property(x => x.FileSizeMb).HasColumnType("decimal(10,2)");
            e.Property(x => x.UploadedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<TrainingProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UploadedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Mentorship>(e =>
        {
            e.ToTable("Mentorships");
            e.HasKey(x => x.MentorshipId);
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Active");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<TrainingProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("Tasks");
            e.HasKey(x => x.TaskId);
            e.Property(x => x.Title).HasMaxLength(255).IsRequired();
            e.Property(x => x.Priority).HasMaxLength(20).HasDefaultValue("Medium");
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Pending");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.AssignedBy).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<TrainingProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DailyLog>(e =>
        {
            e.ToTable("Daily_Logs");
            e.HasKey(x => x.LogId);
            e.Property(x => x.ActivityDescription).IsRequired();
            e.Property(x => x.HoursWorked).HasColumnType("decimal(4,2)");
            e.Property(x => x.KpiScore).HasColumnType("decimal(4,2)");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Assessment>(e =>
        {
            e.ToTable("Assessments");
            e.HasKey(x => x.AssessmentId);
            e.Property(x => x.AssessmentType).HasMaxLength(20).HasDefaultValue("Monthly");
            e.Property(x => x.TechnicalSkillsScore).HasColumnType("decimal(4,2)");
            e.Property(x => x.SoftSkillsScore).HasColumnType("decimal(4,2)");
            e.Property(x => x.CommunicationScore).HasColumnType("decimal(4,2)");
            e.Property(x => x.TeamworkScore).HasColumnType("decimal(4,2)");
            e.Property(x => x.OverallRating).HasColumnType("decimal(4,2)");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<TrainingProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Feedback>(e =>
        {
            e.ToTable("Feedback");
            e.HasKey(x => x.FeedbackId);
            e.Property(x => x.FeedbackType).HasMaxLength(30).HasDefaultValue("General");
            e.Property(x => x.IsAnonymous).HasDefaultValue(false);
            e.Property(x => x.SubmittedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Communication>(e =>
        {
            e.ToTable("Communications");
            e.HasKey(x => x.MessageId);
            e.Property(x => x.Subject).HasMaxLength(255);
            e.Property(x => x.MessageContent).IsRequired();
            e.Property(x => x.IsRead).HasDefaultValue(false);
            e.Property(x => x.SentAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Communication>().WithMany().HasForeignKey(x => x.ParentMessageId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(x => x.NotificationId);
            e.Property(x => x.NotificationType).HasMaxLength(20).HasDefaultValue("In-App");
            e.Property(x => x.Category).HasMaxLength(20).HasDefaultValue("System");
            e.Property(x => x.Subject).HasMaxLength(255).IsRequired();
            e.Property(x => x.Content).IsRequired();
            e.Property(x => x.RelatedType).HasMaxLength(50);
            e.Property(x => x.IsRead).HasDefaultValue(false);
            e.Property(x => x.SentAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Report>(e =>
        {
            e.ToTable("Reports");
            e.HasKey(x => x.ReportId);
            e.Property(x => x.ReportName).HasMaxLength(255).IsRequired();
            e.Property(x => x.ReportType).HasMaxLength(30).HasDefaultValue("Custom");
            e.Property(x => x.FileUrl).HasMaxLength(255);
            e.Property(x => x.FileFormat).HasMaxLength(20).HasDefaultValue("PDF");
            e.Property(x => x.Parameters).HasColumnType("nvarchar(max)");
            e.Property(x => x.GeneratedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.GeneratedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemLog>(e =>
        {
            e.ToTable("System_Logs");
            e.HasKey(x => x.LogId);
            e.Property(x => x.Action).HasMaxLength(255).IsRequired();
            e.Property(x => x.TableAffected).HasMaxLength(100);
            e.Property(x => x.IpAddress).HasMaxLength(45);
            e.Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemSetting>(e =>
        {
            e.ToTable("System_Settings");
            e.HasKey(x => x.SettingId);
            e.Property(x => x.SettingKey).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.SettingKey).IsUnique();
            e.Property(x => x.DataType).HasMaxLength(20).HasDefaultValue("String");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Attendance>(e =>
        {
            e.ToTable("Attendance");
            e.HasKey(x => x.AttendanceId);
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Present");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Certificate>(e =>
        {
            e.ToTable("Certificates");
            e.HasKey(x => x.CertificateId);
            e.Property(x => x.CertificateName).HasMaxLength(255).IsRequired();
            e.Property(x => x.CertificateUrl).HasMaxLength(255);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne<User>().WithMany().HasForeignKey(x => x.InternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<TrainingProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.IssuedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.ReplacedByToken).HasMaxLength(256);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
