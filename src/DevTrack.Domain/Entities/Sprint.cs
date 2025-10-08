using System;

namespace DevTrack.Domain.Entities;

public class Sprint
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; }
    public string Goal { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public SprintStatus Status { get; private set; }

    private Sprint() { }

    public Sprint(Guid projectId, string name, string goal, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Goal = goal;
        StartDate = startDate;
        EndDate = endDate;
        Status = SprintStatus.Planned;

        if (startDate >= endDate)
            throw new ArgumentException("End date must be after start date");
    }

    public void Start()
    {
        if (Status != SprintStatus.Planned)
            throw new InvalidOperationException("Only planned sprints can be started");

        if (DateTime.UtcNow < StartDate)
            throw new InvalidOperationException("Cannot start sprint before its start date");

        Status = SprintStatus.Active;
    }

    public void Complete()
    {
        if (Status != SprintStatus.Active)
            throw new InvalidOperationException("Only active sprints can be completed");

        Status = SprintStatus.Completed;
    }
}

public enum SprintStatus
{
    Planned,
    Active,
    Completed
}