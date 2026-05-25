using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record InternProfileDto(
    int InternId,
    int UserId,
    DateOnly? DateOfBirth,
    string? Address,
    string? University,
    string? Major,
    int? GraduationYear,
    string? EducationalBackground,
    string? WorkHistory,
    string? Skills,
    string? CvUrl,
    string? LinkedinUrl,
    string? GithubUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record InternProfileDetailDto(
    int InternId,
    int UserId,
    string FullName,
    string Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? Address,
    string? University,
    string? Major,
    int? GraduationYear,
    string? EducationalBackground,
    string? WorkHistory,
    string? Skills,
    string? CvUrl,
    string? LinkedinUrl,
    string? GithubUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateInternProfileRequest(
    int UserId,
    DateOnly? DateOfBirth,
    string? Address,
    string? University,
    string? Major,
    int? GraduationYear,
    string? EducationalBackground,
    string? WorkHistory,
    string? Skills,
    string? CvUrl,
    string? LinkedinUrl,
    string? GithubUrl);

public record UpdateInternProfileRequest(
    DateOnly? DateOfBirth,
    string? Address,
    string? University,
    string? Major,
    int? GraduationYear,
    string? EducationalBackground,
    string? WorkHistory,
    string? Skills,
    string? CvUrl,
    string? LinkedinUrl,
    string? GithubUrl);

public record InternProfileFilter(string? Search, string? University, string? Major, int? GraduationYear);

public static class InternProfileDtoExtensions
{
    public static InternProfileDto ToDto(this InternProfile entity) => new(
        entity.InternId, entity.UserId, entity.DateOfBirth, entity.Address, entity.University, entity.Major,
        entity.GraduationYear, entity.EducationalBackground, entity.WorkHistory, entity.Skills,
        entity.CvUrl, entity.LinkedinUrl, entity.GithubUrl, entity.CreatedAt, entity.UpdatedAt);

    public static InternProfile ToEntity(this CreateInternProfileRequest dto) => new()
    {
        UserId = dto.UserId,
        DateOfBirth = dto.DateOfBirth, Address = dto.Address, University = dto.University,
        Major = dto.Major, GraduationYear = dto.GraduationYear, EducationalBackground = dto.EducationalBackground,
        WorkHistory = dto.WorkHistory, Skills = dto.Skills, CvUrl = dto.CvUrl,
        LinkedinUrl = dto.LinkedinUrl, GithubUrl = dto.GithubUrl
    };
}
