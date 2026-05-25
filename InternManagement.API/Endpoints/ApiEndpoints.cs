namespace InternManagement.API.Endpoints;

/// <summary>
/// Central place to register all API endpoint groups
/// </summary>
public static class ApiEndpoints
{
    /// <summary>
    /// Maps all CRUD endpoints to the WebApplication
    /// Call this in Program.cs after adding authentication/authorization
    /// </summary>
    public static void MapAllEndpoints(this WebApplication app)
    {
        // Core Management
        app.MapRoleEndpoints();
        app.MapUserEndpoints();
        app.MapInternProfileEndpoints();

        // Recruitment
        app.MapInternshipCampaignEndpoints();
        app.MapCampaignApplicationEndpoints();
        app.MapInterviewEndpoints();

        // Training
        app.MapTrainingProgramEndpoints();
        app.MapLearningResourceEndpoints();

        // Internship Tracking
        app.MapMentorshipEndpoints();
        app.MapTaskItemEndpoints();
        app.MapDailyLogEndpoints();

        // Evaluation
        app.MapAssessmentEndpoints();
        app.MapFeedbackEndpoints();

        // Communication
        app.MapCommunicationEndpoints();
        app.MapNotificationEndpoints();

        // Admin
        app.MapReportEndpoints();
        app.MapAttendanceEndpoints();
        app.MapCertificateEndpoints();
        app.MapDepartmentEndpoints();

        // External Integrations
        app.MapCalendarEndpoints();
        app.MapZoomEndpoints();
    }
}
