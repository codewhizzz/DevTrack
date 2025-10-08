using System;

namespace DevTrack.Domain.Entities;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid IssueId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public bool IsEdited => EditedAt.HasValue;

    private Comment() { }

    public Comment(Guid issueId, Guid authorId, string content)
    {
        Id = Guid.NewGuid();
        IssueId = issueId;
        AuthorId = authorId;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent ?? throw new ArgumentNullException(nameof(newContent));
        EditedAt = DateTime.UtcNow;
    }
}