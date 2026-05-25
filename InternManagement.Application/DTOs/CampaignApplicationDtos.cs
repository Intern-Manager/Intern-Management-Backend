using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record CampaignApplicationDto(
    int ApplicationId,
    int CampaignId,
    string? CampaignTitle,
    string ApplicantEmail,
    string ApplicantName,
    string? ApplicantPhone,
    string? CvUrl,
    string? CoverLetter,
    string Status,
    DateTime AppliedDate,
    int? ReviewedBy,
    DateTime? ReviewedDate,
    string? Notes);

public record CampaignApplicationDetailDto(
    int ApplicationId,
    int CampaignId,
    string? CampaignTitle,
    string ApplicantEmail,
    string ApplicantName,
    string? ApplicantPhone,
    string? CvUrl,
    string? CoverLetter,
    string Status,
    DateTime AppliedDate,
    int? ReviewedBy,
    string? ReviewedByName,
    DateTime? ReviewedDate,
    string? Notes);

public record CreateCampaignApplicationRequest(
    int CampaignId,
    string ApplicantEmail,
    string ApplicantName,
    string? ApplicantPhone,
    string? CvUrl,
    string? CoverLetter);

public record UpdateCampaignApplicationRequest(
    string? Status,
    int? ReviewedBy,
    string? Notes);

public record CampaignApplicationFilter(
    string? Search,
    string? Status,
    int? CampaignId,
    string? ApplicantEmail);

public static class CampaignApplicationDtoExtensions
{
    public static CampaignApplicationDto ToDto(this CampaignApplication entity) => new(
        entity.ApplicationId, entity.CampaignId, null,
        entity.ApplicantEmail, entity.ApplicantName,
        entity.ApplicantPhone, entity.CvUrl, entity.CoverLetter, entity.Status, entity.AppliedDate,
        entity.ReviewedBy, entity.ReviewedDate, entity.Notes);

    public static CampaignApplication ToEntity(this CreateCampaignApplicationRequest dto) => new()
    {
        CampaignId = dto.CampaignId, ApplicantEmail = dto.ApplicantEmail, ApplicantName = dto.ApplicantName,
        ApplicantPhone = dto.ApplicantPhone, CvUrl = dto.CvUrl, CoverLetter = dto.CoverLetter
    };
}
