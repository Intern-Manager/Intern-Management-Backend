using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;

namespace InternManagement.Application.Services;

public class TaskSubmissionService : ITaskSubmissionService
{
    private readonly ITaskSubmissionRepository _repository;
    private readonly ITaskItemRepository _taskRepository;

    public TaskSubmissionService(ITaskSubmissionRepository repository, ITaskItemRepository taskRepository)
    {
        _repository = repository;
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskSubmissionDto>> GetByTaskIdAsync(int taskId, CancellationToken ct = default)
        => await _repository.GetByTaskIdAsync(taskId, ct);

    public async Task<TaskSubmissionDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetByIdAsync(id, ct);

    public async Task<TaskSubmissionDto?> CreateAsync(CreateTaskSubmissionRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        await _repository.AddAsync(entity, ct);

        // Update task status to InProgress
        var task = await _taskRepository.GetByIdAsync(request.TaskId, ct);
        if (task is not null)
        {
            task.Status = "InProgress";
            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task, ct);
        }

        return await _repository.GetByIdAsync(entity.SubmissionId, ct);
    }

    public async Task<TaskSubmissionDto?> GradeAsync(int id, int gradedBy, GradeSubmissionRequest request, CancellationToken ct = default)
    {
        // Get entity from base repository
        var entity = await ((IGenericRepository<TaskSubmission>)_repository).GetByIdAsync(id, ct);
        if (entity is null) return null;

        entity.Score = request.Score;
        entity.Feedback = request.Feedback;
        entity.GradedBy = gradedBy;
        entity.GradedAt = DateTime.UtcNow;
        entity.Status = "Graded";

        await _repository.UpdateAsync(entity, ct);

        // Update task status to Completed
        var task = await _taskRepository.GetByIdAsync(entity.TaskId, ct);
        if (task is not null)
        {
            task.Status = "Completed";
            task.CompletionDate = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task, ct);
        }

        // Return updated DTO
        return await _repository.GetByIdAsync(id, ct);
    }
}
