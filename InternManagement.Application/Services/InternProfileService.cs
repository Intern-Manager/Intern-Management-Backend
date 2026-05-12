using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class InternProfileService : IInternProfileService
{
    private readonly IInternProfileRepository _repository;

    public InternProfileService(IInternProfileRepository repository) => _repository = repository;

    public async Task<PaginatedResult<InternProfileDto>> GetAllAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InternProfileDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InternProfileDto?> CreateAsync(int userId, CreateInternProfileRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InternProfileDto?> UpdateAsync(int id, UpdateInternProfileRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.DateOfBirth.HasValue) entity.DateOfBirth = request.DateOfBirth;
        if (request.Address is not null) entity.Address = request.Address;
        if (request.University is not null) entity.University = request.University;
        if (request.Major is not null) entity.Major = request.Major;
        if (request.GraduationYear.HasValue) entity.GraduationYear = request.GraduationYear;
        if (request.EducationalBackground is not null) entity.EducationalBackground = request.EducationalBackground;
        if (request.WorkHistory is not null) entity.WorkHistory = request.WorkHistory;
        if (request.Skills is not null) entity.Skills = request.Skills;
        if (request.CvUrl is not null) entity.CvUrl = request.CvUrl;
        if (request.LinkedinUrl is not null) entity.LinkedinUrl = request.LinkedinUrl;
        if (request.GithubUrl is not null) entity.GithubUrl = request.GithubUrl;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
