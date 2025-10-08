using System;
using System.Collections.Generic;
using System.Linq;

namespace DevTrack.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Key { get; private set; } // e.g., "PROJ" for issue keys like PROJ-123
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

    private readonly List<Issue> _issues = new();
    public IReadOnlyCollection<Issue> Issues => _issues.AsReadOnly();

    private readonly List<Sprint> _sprints = new();
    public IReadOnlyCollection<Sprint> Sprints => _sprints.AsReadOnly();

    private Project() { }

    public Project(string name, string description, string key, Guid createdBy)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Key = key?.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(key));
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;

        // Creator is automatically added as a maintainer
        AddMember(createdBy, ProjectRole.Maintainer);
    }

    public void AddMember(Guid userId, ProjectRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already a member of this project");

        _members.Add(new ProjectMember(Id, userId, role));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            throw new InvalidOperationException("User is not a member of this project");

        if (_members.Count(m => m.Role == ProjectRole.Maintainer) == 1 && member.Role == ProjectRole.Maintainer)
            throw new InvalidOperationException("Cannot remove the last maintainer from the project");

        _members.Remove(member);
    }

    public void UpdateMemberRole(Guid userId, ProjectRole newRole)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            throw new InvalidOperationException("User is not a member of this project");

        member.UpdateRole(newRole);
    }

    public bool IsMember(Guid userId) => _members.Any(m => m.UserId == userId);
    
    public bool HasRole(Guid userId, ProjectRole role) => 
        _members.Any(m => m.UserId == userId && m.Role == role);

    public void Deactivate()
    {
        IsActive = false;
    }
}

public enum ProjectRole
{
    Viewer = 0,
    Developer = 1,
    Maintainer = 2
}