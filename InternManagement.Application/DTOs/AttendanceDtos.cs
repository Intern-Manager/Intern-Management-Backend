using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record AttendanceDto(
    int AttendanceId,
    int InternId,
    DateOnly AttendanceDate,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string Status,
    string? Notes,
    int? ApprovedBy,
    DateTime CreatedAt);

public record AttendanceDetailDto(
    int AttendanceId,
    int InternId,
    string InternName,
    DateOnly AttendanceDate,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string Status,
    string? Notes,
    int? ApprovedBy,
    string? ApprovedByName,
    DateTime CreatedAt);

public record CreateAttendanceRequest(
    int InternId,
    DateOnly AttendanceDate,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string Status,
    string? Notes);

public record UpdateAttendanceRequest(
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string? Status,
    string? Notes,
    int? ApprovedBy);

public record AttendanceFilter(
    int? InternId,
    string? Status,
    DateOnly? FromDate,
    DateOnly? ToDate);

public static class AttendanceDtoExtensions
{
    public static AttendanceDto ToDto(this Attendance entity) => new(
        entity.AttendanceId, entity.InternId, entity.AttendanceDate, entity.CheckInTime,
        entity.CheckOutTime, entity.Status, entity.Notes, entity.ApprovedBy, entity.CreatedAt);

    public static Attendance ToEntity(this CreateAttendanceRequest dto) => new()
    {
        InternId = dto.InternId, AttendanceDate = dto.AttendanceDate, CheckInTime = dto.CheckInTime,
        CheckOutTime = dto.CheckOutTime, Status = dto.Status, Notes = dto.Notes
    };
}
