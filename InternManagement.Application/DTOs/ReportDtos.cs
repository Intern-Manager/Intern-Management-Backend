using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record ReportDto(
    int ReportId,
    string ReportName,
    string ReportType,
    string? Description,
    int GeneratedBy,
    string? FileUrl,
    string FileFormat,
    string? Parameters,
    DateTime GeneratedAt);

public record ReportDetailDto(
    int ReportId,
    string ReportName,
    string ReportType,
    string? Description,
    int GeneratedBy,
    string GeneratedByName,
    string? FileUrl,
    string FileFormat,
    string? Parameters,
    DateTime GeneratedAt);

public record CreateReportRequest(
    string ReportName,
    string ReportType,
    string? Description,
    int GeneratedBy,
    string? FileUrl,
    string FileFormat,
    string? Parameters);

public record UpdateReportRequest(
    string? ReportName,
    string? ReportType,
    string? Description,
    string? FileUrl,
    string? FileFormat,
    string? Parameters);

public record ReportFilter(
    string? Search,
    string? ReportType);

public static class ReportDtoExtensions
{
    public static ReportDto ToDto(this Report entity) => new(
        entity.ReportId, entity.ReportName, entity.ReportType, entity.Description,
        entity.GeneratedBy, entity.FileUrl, entity.FileFormat, entity.Parameters, entity.GeneratedAt);

    public static Report ToEntity(this CreateReportRequest dto) => new()
    {
        ReportName = dto.ReportName, ReportType = dto.ReportType, Description = dto.Description,
        GeneratedBy = dto.GeneratedBy, FileUrl = dto.FileUrl, FileFormat = dto.FileFormat,
        Parameters = dto.Parameters
    };
}
