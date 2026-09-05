namespace Master.Domain.Enums;

/// <summary>
/// Enumeration Class pattern for MasterStatus adhering to SOLID / Open-Closed Principle (OCP).
/// Provides rich status metadata (Id, Name, DisplayName, ColorCode, CanAcceptJobs) in standard English.
/// </summary>
public class MasterStatus : IComparable<MasterStatus>, IEquatable<MasterStatus>
{
    public static readonly MasterStatus Available = new(1, nameof(Available), "Available", "#22C55E", canAcceptJobs: true);
    public static readonly MasterStatus Busy = new(2, nameof(Busy), "Busy", "#F59E0B", canAcceptJobs: false);
    public static readonly MasterStatus Offline = new(3, nameof(Offline), "Offline", "#6B7280", canAcceptJobs: false);

    public int Id { get; }
    public string Name { get; }
    public string DisplayName { get; }
    public string ColorCode { get; }
    public bool CanAcceptJobs { get; }

    public MasterStatus() : this(1, nameof(Available), "Available", "#22C55E", true) { }

    public MasterStatus(int id, string name, string displayName, string colorCode, bool canAcceptJobs)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        ColorCode = colorCode;
        CanAcceptJobs = canAcceptJobs;
    }

    public static IEnumerable<MasterStatus> GetAll() =>
        new[] { Available, Busy, Offline };

    public static MasterStatus FromId(int id)
    {
        var status = GetAll().FirstOrDefault(s => s.Id == id);
        return status ?? throw new ArgumentOutOfRangeException(nameof(id), $"Invalid MasterStatus Id: {id}");
    }

    public static MasterStatus FromName(string name)
    {
        var status = GetAll().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        return status ?? throw new ArgumentOutOfRangeException(nameof(name), $"Invalid MasterStatus Name: {name}");
    }

    public static MasterStatus? TryFromId(int? id)
    {
        return id.HasValue ? GetAll().FirstOrDefault(s => s.Id == id.Value) : null;
    }

    public static MasterStatus? TryFromName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return GetAll().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj) =>
        obj is MasterStatus other && Equals(other);

    public bool Equals(MasterStatus? other) =>
        other is not null && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(MasterStatus? other) =>
        other is null ? 1 : Id.CompareTo(other.Id);

    public static bool operator ==(MasterStatus? left, MasterStatus? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(MasterStatus? left, MasterStatus? right) => !(left == right);
}
