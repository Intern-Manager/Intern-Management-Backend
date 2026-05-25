using System.Net;
using System.Net.Mail;
using InternManagement.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InternManagement.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration["SendGrid:ApiKey"] ?? throw new ArgumentNullException("SendGrid:ApiKey");
        _fromEmail = configuration["SendGrid:FromEmail"] ?? throw new ArgumentNullException("SendGrid:FromEmail");
        _fromName = configuration["SendGrid:FromName"] ?? "Intern Management System";
    }

    public async Task<bool> SendEmailAsync(EmailRequest request, CancellationToken ct = default)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var payload = new
            {
                personalizations = new[]
                {
                    new
                    {
                        to = new[] { new { email = request.ToEmail, name = request.ToName } },
                        subject = request.Subject
                    }
                },
                from = new { email = _fromEmail, name = _fromName },
                content = new[]
                {
                    new { type = "text/html", value = request.HtmlContent }
                }
            };

            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content, ct);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {Email}", request.ToEmail);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Failed to send email to {Email}. Status: {Status}, Error: {Error}",
                request.ToEmail, response.StatusCode, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email to {Email}", request.ToEmail);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string email, string name, string tempPassword, CancellationToken ct = default)
    {
        var subject = "Welcome to Intern Management System!";
        var html = GetWelcomeEmailTemplate(name, tempPassword);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = email,
            ToName = name,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.Welcome
        }, ct);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string name, string resetLink, CancellationToken ct = default)
    {
        var subject = "Reset Your Password - Intern Management System";
        var html = GetPasswordResetTemplate(name, resetLink);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = email,
            ToName = name,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.PasswordReset
        }, ct);
    }

    public async Task<bool> SendInterviewInvitationAsync(InterviewEmailData data, CancellationToken ct = default)
    {
        var subject = $"Interview Invitation - {data.Position}";
        var html = GetInterviewInvitationTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.CandidateEmail ?? throw new ArgumentNullException("CandidateEmail"),
            ToName = data.CandidateName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.InterviewInvitation
        }, ct);
    }

    public async Task<bool> SendInterviewReminderAsync(InterviewEmailData data, CancellationToken ct = default)
    {
        var subject = $"Reminder: Interview Tomorrow - {data.Position}";
        var html = GetInterviewReminderTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.CandidateEmail ?? throw new ArgumentNullException("CandidateEmail"),
            ToName = data.CandidateName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.InterviewReminder
        }, ct);
    }

    public async Task<bool> SendApplicationReceivedEmailAsync(ApplicationEmailData data, CancellationToken ct = default)
    {
        var subject = $"Application Received - {data.CampaignTitle}";
        var html = GetApplicationReceivedTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.ApplicantEmail ?? throw new ArgumentNullException("ApplicantEmail"),
            ToName = data.ApplicantName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.ApplicationReceived
        }, ct);
    }

    public async Task<bool> SendApplicationStatusUpdateEmailAsync(ApplicationEmailData data, CancellationToken ct = default)
    {
        var subject = $"Application Status Update - {data.CampaignTitle}";
        var html = GetApplicationStatusUpdateTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.ApplicantEmail ?? throw new ArgumentNullException("ApplicantEmail"),
            ToName = data.ApplicantName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.ApplicationStatusUpdate
        }, ct);
    }

    public async Task<bool> SendTrainingEnrollmentEmailAsync(TrainingEmailData data, CancellationToken ct = default)
    {
        var subject = $"You've Been Enrolled in {data.ProgramName}";
        var html = GetTrainingEnrollmentTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.InternEmail ?? throw new ArgumentNullException("InternEmail"),
            ToName = data.InternName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.TrainingEnrollment
        }, ct);
    }

    public async Task<bool> SendAssessmentNotificationAsync(string email, string internName, string assessmentType, DateTime dueDate, CancellationToken ct = default)
    {
        var subject = $"Assessment Due: {assessmentType}";
        var html = GetAssessmentNotificationTemplate(internName, assessmentType, dueDate);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = email,
            ToName = internName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.AssessmentNotification
        }, ct);
    }

    public async Task<bool> SendDailyLogReminderAsync(string email, string internName, CancellationToken ct = default)
    {
        var subject = "Reminder: Submit Your Daily Log";
        var html = GetDailyLogReminderTemplate(internName);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = email,
            ToName = internName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.DailyLogReminder
        }, ct);
    }

    public async Task<bool> SendNewTaskEmailAsync(TaskEmailData data, CancellationToken ct = default)
    {
        var subject = $"New Task Assigned: {data.TaskTitle}";
        var html = GetNewTaskTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.InternEmail ?? throw new ArgumentNullException("InternEmail"),
            ToName = data.InternName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.NewTaskAssigned
        }, ct);
    }

    public async Task<bool> SendTaskDeadlineReminderAsync(TaskEmailData data, CancellationToken ct = default)
    {
        var subject = $"Task Deadline Approaching: {data.TaskTitle}";
        var html = GetTaskDeadlineReminderTemplate(data);

        return await SendEmailAsync(new EmailRequest
        {
            ToEmail = data.InternEmail ?? throw new ArgumentNullException("InternEmail"),
            ToName = data.InternName,
            Subject = subject,
            HtmlContent = html,
            TemplateType = EmailTemplateType.TaskDeadlineReminder
        }, ct);
    }

    #region HTML Email Templates

    private static string GetEmailBaseTemplate(string title, string content)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .email-wrapper {{ background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .content {{ padding: 30px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 10px 0; }}
        .button-outline {{ display: inline-block; padding: 12px 30px; border: 2px solid #667eea; color: #667eea; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 10px 0; }}
        .info-box {{ background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 15px; margin: 15px 0; border-radius: 0 5px 5px 0; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        ul {{ padding-left: 20px; }}
        li {{ margin: 8px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='email-wrapper'>
            <div class='header'>
                <h1>{title}</h1>
            </div>
            <div class='content'>
                {content}
            </div>
            <div class='footer'>
                <p>Intern Management System</p>
                <p>If you have any questions, please contact our support team.</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetWelcomeEmailTemplate(string name, string tempPassword)
    {
        var content = $@"
<h2>Welcome, {name}!</h2>
<p>Congratulations! Your account has been successfully created in our Intern Management System.</p>

<div class='info-box'>
    <h4>Your Login Credentials:</h4>
    <p><strong>Email:</strong> [Your Email]</p>
    <p><strong>Temporary Password:</strong> <span class='highlight'>{tempPassword}</span></p>
</div>

<p><strong>Important:</strong> Please change your password after your first login for security reasons.</p>

<p>Here's what you can do with your new account:</p>
<ul>
    <li>Complete your profile</li>
    <li>View and submit daily logs</li>
    <li>Access training materials</li>
    <li>Track your tasks and assignments</li>
    <li>Communicate with your mentor</li>
</ul>

<p style='text-align: center;'>
    <a href='#' class='button'>Login to Your Account</a>
</p>

<p>We wish you a great internship experience!</p>";
        return GetEmailBaseTemplate("Welcome to Intern Management System", content);
    }

    private static string GetPasswordResetTemplate(string name, string resetLink)
    {
        var content = $@"
<h2>Hello, {name}!</h2>
<p>We received a request to reset your password. Click the button below to reset it:</p>

<p style='text-align: center;'>
    <a href='{resetLink}' class='button'>Reset Password</a>
</p>

<div class='info-box'>
    <p><strong>Note:</strong> This link will expire in 24 hours.</p>
    <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
</div>

<p>For security reasons, please don't share this link with anyone.</p>";
        return GetEmailBaseTemplate("Reset Your Password", content);
    }

    private static string GetInterviewInvitationTemplate(InterviewEmailData data)
    {
        var meetingInfo = !string.IsNullOrEmpty(data.MeetingLink)
            ? $"<p><strong>Meeting Link:</strong> <a href='{data.MeetingLink}'>{data.MeetingLink}</a></p>"
            : $"<p><strong>Location:</strong> {data.Location ?? "To be confirmed"}</p>";

        var content = $@"
<h2>Interview Invitation</h2>
<p>Dear <strong>{data.CandidateName}</strong>,</p>

<p>We are pleased to invite you for an interview for the <span class='highlight'>{data.Position}</span> position.</p>

<div class='info-box'>
    <h4>Interview Details:</h4>
    <p><strong>Date:</strong> {data.InterviewDate:dddd, MMMM dd, yyyy}</p>
    <p><strong>Time:</strong> {data.StartTime}</p>
    <p><strong>Duration:</strong> {data.Duration}</p>
    <p><strong>Type:</strong> {data.InterviewType}</p>
    {meetingInfo}
    <p><strong>Interviewer:</strong> {data.InterviewerName}</p>
</div>

<p>Please confirm your attendance by replying to this email.</p>

<p>If you have any questions, feel free to contact us.</p>

<p>We look forward to meeting you!</p>";
        return GetEmailBaseTemplate($"Interview Invitation - {data.Position}", content);
    }

    private static string GetInterviewReminderTemplate(InterviewEmailData data)
    {
        var meetingInfo = !string.IsNullOrEmpty(data.MeetingLink)
            ? $"<p><strong>Meeting Link:</strong> <a href='{data.MeetingLink}'>{data.MeetingLink}</a></p>"
            : "";

        var content = $@"
<h2>Interview Reminder</h2>
<p>Dear <strong>{data.CandidateName}</strong>,</p>

<p>This is a friendly reminder about your upcoming interview:</p>

<div class='info-box'>
    <h4>Tomorrow - {data.InterviewDate:MMM dd, yyyy}</h4>
    <p><strong>Time:</strong> {data.StartTime}</p>
    <p><strong>Position:</strong> {data.Position}</p>
    {meetingInfo}
</div>

<p>Please make sure to:</p>
<ul>
    <li>Join 5-10 minutes early</li>
    <li>Have your documents ready</li>
    <li>Test your audio/video if it's a video interview</li>
</ul>

<p>See you tomorrow!</p>";
        return GetEmailBaseTemplate("Interview Reminder - Tomorrow", content);
    }

    private static string GetApplicationReceivedTemplate(ApplicationEmailData data)
    {
        var content = $@"
<h2>Application Received!</h2>
<p>Dear <strong>{data.ApplicantName}</strong>,</p>

<p>Thank you for applying to <span class='highlight'>{data.CampaignTitle}</span>!</p>

<div class='info-box'>
    <p>We've received your application and our team will review it shortly.</p>
    <p>You'll receive an email update once the review is complete.</p>
</div>

<p>In the meantime, you can:</p>
<ul>
    <li>Complete your profile</li>
    <li>Prepare for potential interviews</li>
    <li>Explore our company</li>
</ul>

<p>We appreciate your interest in joining our team!</p>";
        return GetEmailBaseTemplate("Application Received", content);
    }

    private static string GetApplicationStatusUpdateTemplate(ApplicationEmailData data)
    {
        var statusColor = data.Status.ToLower() switch
        {
            "approved" or "accepted" or "selected" => "#28a745",
            "rejected" or "declined" => "#dc3545",
            "pending" or "under review" => "#ffc107",
            _ => "#667eea"
        };

        var content = $@"
<h2>Application Status Update</h2>
<p>Dear <strong>{data.ApplicantName}</strong>,</p>

<p>Your application for <span class='highlight'>{data.CampaignTitle}</span> has been updated.</p>

<div class='info-box'>
    <h4 style='color: {statusColor};'>Status: {data.Status}</h4>
    {(string.IsNullOrEmpty(data.StatusMessage) ? "" : $"<p>{data.StatusMessage}</p>")}
    {(string.IsNullOrEmpty(data.NextSteps) ? "" : $"<p><strong>Next Steps:</strong> {data.NextSteps}</p>")}
</div>

<p>We will continue to keep you updated on your application status.</p>";
        return GetEmailBaseTemplate("Application Status Update", content);
    }

    private static string GetTrainingEnrollmentTemplate(TrainingEmailData data)
    {
        var content = $@"
<h2>Training Program Enrollment</h2>
<p>Dear <strong>{data.InternName}</strong>,</p>

<p>You have been enrolled in a new training program!</p>

<div class='info-box'>
    <h4>{data.ProgramName}</h4>
    <p><strong>Start Date:</strong> {data.StartDate:MMMM dd, yyyy}</p>
    <p><strong>End Date:</strong> {data.EndDate:MMMM dd, yyyy}</p>
    <p><strong>Schedule:</strong> {data.Schedule}</p>
    {(string.IsNullOrEmpty(data.Description) ? "" : $"<p><strong>Description:</strong> {data.Description}</p>")}
</div>

<p>Please log in to access the training materials and start learning!</p>

<p style='text-align: center;'>
    <a href='#' class='button'>Access Training</a>
</p>

<p>Best of luck with your training!</p>";
        return GetEmailBaseTemplate("Training Program Enrollment", content);
    }

    private static string GetAssessmentNotificationTemplate(string internName, string assessmentType, DateTime dueDate)
    {
        var content = $@"
<h2>Assessment Notification</h2>
<p>Dear <strong>{internName}</strong>,</p>

<p>This is a reminder about an upcoming assessment.</p>

<div class='info-box'>
    <h4>Assessment: {assessmentType}</h4>
    <p><strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
    <p><strong>Time Remaining:</strong> {(dueDate - DateTime.Now).Days} days</p>
</div>

<p>Please make sure to:</p>
<ul>
    <li>Review all materials before the assessment</li>
    <li>Submit before the deadline</li>
    <li>Contact your mentor if you have any questions</li>
</ul>

<p style='text-align: center;'>
    <a href='#' class='button'>View Assessment Details</a>
</p>";
        return GetEmailBaseTemplate($"Assessment Due: {assessmentType}", content);
    }

    private static string GetDailyLogReminderTemplate(string internName)
    {
        var content = $@"
<h2>Daily Log Reminder</h2>
<p>Hi <strong>{internName}</strong>,</p>

<p>This is a friendly reminder to submit your daily log for today.</p>

<div class='info-box'>
    <p>Regular daily log submissions help you track your progress and receive valuable feedback from your mentor.</p>
</div>

<p style='text-align: center;'>
    <a href='#' class='button'>Submit Daily Log</a>
</p>

<p>Don't forget - consistent logging shows your dedication and growth!</p>";
        return GetEmailBaseTemplate("Reminder: Submit Your Daily Log", content);
    }

    private static string GetNewTaskTemplate(TaskEmailData data)
    {
        var priorityColor = data.Priority.ToLower() switch
        {
            "high" or "urgent" => "#dc3545",
            "medium" => "#ffc107",
            "low" => "#28a745",
            _ => "#667eea"
        };

        var content = $@"
<h2>New Task Assigned</h2>
<p>Hello <strong>{data.InternName}</strong>,</p>

<p>You have a new task assigned by <strong>{data.AssignedByName}</strong>.</p>

<div class='info-box'>
    <h4 style='color: {priorityColor};'>{data.TaskTitle}</h4>
    <p><strong>Due Date:</strong> {data.DueDate}</p>
    <p><strong>Priority:</strong> <span style='color: {priorityColor};'>{data.Priority}</span></p>
    {(string.IsNullOrEmpty(data.Description) ? "" : $"<p><strong>Description:</strong> {data.Description}</p>")}
</div>

<p style='text-align: center;'>
    <a href='#' class='button'>View Task Details</a>
</p>

<p>Good luck with your task!</p>";
        return GetEmailBaseTemplate($"New Task: {data.TaskTitle}", content);
    }

    private static string GetTaskDeadlineReminderTemplate(TaskEmailData data)
    {
        var content = $@"
<h2>Task Deadline Approaching!</h2>
<p>Hi <strong>{data.InternName}</strong>,</p>

<p>This is a reminder that the following task is due soon:</p>

<div class='info-box'>
    <h4>{data.TaskTitle}</h4>
    <p><strong>Due Date:</strong> {data.DueDate}</p>
    <p><strong>Priority:</strong> {data.Priority}</p>
</div>

<p>Please make sure to complete and submit your work on time.</p>

<p style='text-align: center;'>
    <a href='#' class='button'>View Task</a>
</p>

<p>If you need an extension, please contact your mentor before the deadline.</p>";
        return GetEmailBaseTemplate($"Deadline Approaching: {data.TaskTitle}", content);
    }

    #endregion
}
