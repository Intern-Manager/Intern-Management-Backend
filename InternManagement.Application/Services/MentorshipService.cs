using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class MentorshipService : IMentorshipService
{
    private readonly IMentorshipRepository _repository;
    private readonly INotificationService _notificationService;

    public MentorshipService(IMentorshipRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task<PaginatedResult<MentorshipDto>> GetAllAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<MentorshipDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<MentorshipDto?> CreateAsync(CreateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);

        // Notify intern about internship assignment
        try
        {
            await _notificationService.CreateAsync(new CreateNotificationRequest(
                request.InternId, "In-App", "Internship",
                "Welcome to Your Internship!",
                $"You have been assigned to the program starting {request.StartDate:MMM d, yyyy}." +
                $" Please check your schedule and reach out to your mentor for onboarding.",
                entity.MentorshipId, "Mentorship"), ct);

            // Also notify mentor
            await _notificationService.CreateAsync(new CreateNotificationRequest(
                request.MentorId, "In-App", "Internship",
                "New Intern Assigned",
                $"A new intern has been assigned to your mentorship. Please review their profile and get in touch.",
                entity.MentorshipId, "Mentorship"), ct);
        }
        catch { /* don't fail if notification fails */ }

        return entity.ToDto();
    }

    public async Task<MentorshipDto?> UpdateAsync(int id, UpdateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.MentorId.HasValue) entity.MentorId = request.MentorId.Value;
        if (request.InternId.HasValue) entity.InternId = request.InternId.Value;
        if (request.ProgramId.HasValue) entity.ProgramId = request.ProgramId.Value;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.Status is not null) entity.Status = request.Status;

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
