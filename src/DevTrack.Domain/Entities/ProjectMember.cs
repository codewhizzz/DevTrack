using System;

namespace DevTrack.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public ProjectRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private ProjectMember() { }

    public ProjectMember(Guid projectId, Guid userId, ProjectRole role)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateRole(ProjectRole newRole)
    {
        Role = newRole;
    }
}