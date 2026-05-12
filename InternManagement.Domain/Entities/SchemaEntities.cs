using System.ComponentModel.DataAnnotations;

namespace InternManagement.Domain.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public int RoleId { get; set; }
    public string Status { get; set; } = "Inactive";
    public bool EmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpires { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class InternProfile
{
    public int InternId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? University { get; set; }
    public string? Major { get; set; }
    public int? GraduationYear { get; set; }
    public string? EducationalBackground { get; set; }
    public string? WorkHistory { get; set; }
    public string? Skills { get; set; }
    public string? CvUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? GithubUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InternshipCampaign
{
    public int CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public int NumberOfPositions { get; set; } = 1;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ApplicationDeadline { get; set; }
    public string Status { get; set; } = "Draft";
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CampaignApplication
{
    public int ApplicationId { get; set; }
    public int CampaignId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? ApplicantPhone { get; set; }
    public string? CvUrl { get; set; }
    public string? CoverLetter { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime AppliedDate { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? Notes { get; set; }
}

public class Interview
{
    public int InterviewId { get; set; }
    public int? CampaignId { get; set; }
    public int? ApplicationId { get; set; }
    public int? InternId { get; set; }
    public int InterviewerId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string InterviewType { get; set; } = "Video";
    public string? MeetingLink { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Feedback { get; set; }
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TrainingProgram
{
    public int ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Objectives { get; set; }
    public int? DurationWeeks { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int CoordinatorId { get; set; }
    public string Status { get; set; } = "Planning";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class LearningResource
{
    public int ResourceId { get; set; }
    public int ProgramId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ResourceUrl { get; set; }
    public string ResourceType { get; set; } = "Document";
    public decimal? FileSizeMb { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class Mentorship
{
    public int MentorshipId { get; set; }
    public int MentorId { get; set; }
    public int InternId { get; set; }
    public int ProgramId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
}

public class TaskItem
{
    public int TaskId { get; set; }
    public int InternId { get; set; }
    public int AssignedBy { get; set; }
    public int? ProgramId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Pending";
    public DateTime? CompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DailyLog
{
    public int LogId { get; set; }
    public int InternId { get; set; }
    public int? MentorId { get; set; }
    public DateOnly LogDate { get; set; }
    public string ActivityDescription { get; set; } = string.Empty;
    public decimal? HoursWorked { get; set; }
    public string? ChallengesFaced { get; set; }
    public string? MentorFeedback { get; set; }
    public decimal? KpiScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class Assessment
{
    public int AssessmentId { get; set; }
    public int InternId { get; set; }
    public int MentorId { get; set; }
    public int? ProgramId { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public string AssessmentType { get; set; } = "Monthly";
    public decimal? TechnicalSkillsScore { get; set; }
    public decimal? SoftSkillsScore { get; set; }
    public decimal? CommunicationScore { get; set; }
    public decimal? TeamworkScore { get; set; }
    public decimal? OverallRating { get; set; }
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Feedback
{
    public int FeedbackId { get; set; }
    public int InternId { get; set; }
    public string FeedbackType { get; set; } = "General";
    public int? RelatedId { get; set; }
    public int? Rating { get; set; }
    public string? Comments { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class Communication
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string? Subject { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int? ParentMessageId { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class Notification
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string NotificationType { get; set; } = "In-App";
    public string Category { get; set; } = "System";
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class Report
{
    public int ReportId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = "Custom";
    public string? Description { get; set; }
    public int GeneratedBy { get; set; }
    public string? FileUrl { get; set; }
    public string FileFormat { get; set; } = "PDF";
    public string? Parameters { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class SystemLog
{
    public int LogId { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TableAffected { get; set; }
    public int? RecordId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SystemSetting
{
    public int SettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "String";
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class Attendance
{
    public int AttendanceId { get; set; }
    public int InternId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present";
    public string? Notes { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Certificate
{
    public int CertificateId { get; set; }
    public int InternId { get; set; }
    public int? ProgramId { get; set; }
    public string CertificateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly IssuedDate { get; set; }
    public string? CertificateUrl { get; set; }
    public int IssuedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
