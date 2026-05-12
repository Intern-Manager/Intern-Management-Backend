using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _repository;

    public AssessmentService(IAssessmentRepository repository) => _repository = repository;

    public async Task<PaginatedResult<AssessmentDto>> GetAllAsync(PaginationRequest pagination, AssessmentFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<AssessmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<AssessmentDto?> CreateAsync(CreateAssessmentRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<AssessmentDto?> UpdateAsync(int id, UpdateAssessmentRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.AssessmentDate.HasValue) entity.AssessmentDate = request.AssessmentDate.Value;
        if (request.AssessmentType is not null) entity.AssessmentType = request.AssessmentType;
        if (request.TechnicalSkillsScore.HasValue) entity.TechnicalSkillsScore = request.TechnicalSkillsScore;
        if (request.SoftSkillsScore.HasValue) entity.SoftSkillsScore = request.SoftSkillsScore;
        if (request.CommunicationScore.HasValue) entity.CommunicationScore = request.CommunicationScore;
        if (request.TeamworkScore.HasValue) entity.TeamworkScore = request.TeamworkScore;
        if (request.OverallRating.HasValue) entity.OverallRating = request.OverallRating;
        if (request.Strengths is not null) entity.Strengths = request.Strengths;
        if (request.AreasForImprovement is not null) entity.AreasForImprovement = request.AreasForImprovement;
        if (request.Comments is not null) entity.Comments = request.Comments;

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
