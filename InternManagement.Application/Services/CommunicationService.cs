using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class CommunicationService : ICommunicationService
{
    private readonly ICommunicationRepository _repository;

    public CommunicationService(ICommunicationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CommunicationDto>> GetAllAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CommunicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CommunicationDto?> CreateAsync(CreateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.SentAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CommunicationDto?> UpdateAsync(int id, UpdateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.IsRead.HasValue) entity.IsRead = request.IsRead.Value;
        if (request.ReadAt.HasValue) entity.ReadAt = request.ReadAt;

        await _repository.UpdateAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }

    public async Task<IEnumerable<ChatContactDto>> GetConversationsAsync(int userId, CancellationToken ct = default)
    {
        return await _repository.GetConversationsAsync(userId, ct);
    }

    public async Task<PaginatedResult<CommunicationDto>> GetConversationMessagesAsync(int currentUserId, int otherUserId, PaginationRequest pagination, CancellationToken ct = default)
    {
        var items = await _repository.GetMessagesBetweenUsersAsync(currentUserId, otherUserId, pagination, ct);
        var list = items.ToList();
        return list.ToPaginatedResult(pagination, list.Count);
    }

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
    {
        var filter = new CommunicationFilter(null, userId, false);
        return await _repository.CountAsync(filter, ct);
    }

    public async Task MarkAsReadAsync(int senderId, int receiverId, CancellationToken ct = default)
    {
        await _repository.MarkAllAsReadAsync(senderId, receiverId, ct);
    }

    public async Task<PaginatedResult<CommunicationDto>> GetMyMessagesAsync(int currentUserId, int? otherUserId, PaginationRequest pagination, CancellationToken ct = default)
    {
        var allItems = await _repository.GetAllDtoAsync(new PaginationRequest(1, 1000), null, ct);

        IEnumerable<CommunicationDto> query = allItems
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId);

        if (otherUserId.HasValue)
            query = query.Where(m => m.SenderId == otherUserId.Value || m.ReceiverId == otherUserId.Value);

        var filtered = query
            .OrderBy(m => m.SentAt)
            .ToList();

        var total = filtered.Count;
        var paged = filtered
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return paged.ToPaginatedResult(pagination, total);
    }
}
