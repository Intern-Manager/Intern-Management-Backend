using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record CertificateDto(
    int CertificateId,
    int InternId,
    int? ProgramId,
    string CertificateName,
    string? Description,
    DateOnly IssuedDate,
    string? CertificateUrl,
    int IssuedBy,
    DateTime CreatedAt);

public record CertificateDetailDto(
    int CertificateId,
    int InternId,
    string InternName,
    int? ProgramId,
    string? ProgramName,
    string CertificateName,
    string? Description,
    DateOnly IssuedDate,
    string? CertificateUrl,
    int IssuedBy,
    string IssuedByName,
    DateTime CreatedAt);

public record CreateCertificateRequest(
    int InternId,
    int? ProgramId,
    string CertificateName,
    string? Description,
    DateOnly IssuedDate,
    string? CertificateUrl,
    int IssuedBy);

public record UpdateCertificateRequest(
    string? CertificateName,
    string? Description,
    DateOnly? IssuedDate,
    string? CertificateUrl);

public record CertificateFilter(
    string? Search,
    int? InternId,
    int? ProgramId);

public static class CertificateDtoExtensions
{
    public static CertificateDto ToDto(this Certificate entity) => new(
        entity.CertificateId, entity.InternId, entity.ProgramId, entity.CertificateName,
        entity.Description, entity.IssuedDate, entity.CertificateUrl, entity.IssuedBy, entity.CreatedAt);

    public static Certificate ToEntity(this CreateCertificateRequest dto) => new()
    {
        InternId = dto.InternId, ProgramId = dto.ProgramId, CertificateName = dto.CertificateName,
        Description = dto.Description, IssuedDate = dto.IssuedDate, CertificateUrl = dto.CertificateUrl,
        IssuedBy = dto.IssuedBy
    };
}
