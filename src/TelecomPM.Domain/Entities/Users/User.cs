using Microsoft.AspNetCore.Identity;
using TelecomPM.Domain.Common;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.UserEvents;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.Users;

// ==================== User (Aggregate Root) ====================
public sealed class User : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid OfficeId { get; private set; }
    public bool IsActive { get; private set; }
    public bool MustChangePassword { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    // For PM Engineers
    public int? MaxAssignedSites { get; private set; }
    private readonly List<string> _specializations = new();
    public IReadOnlyCollection<string> Specializations => _specializations.AsReadOnly();
    public decimal? PerformanceRating { get; private set; }
    
    // Assignments
    private readonly List<Guid> _assignedSiteIds = new();
    public IReadOnlyCollection<Guid> AssignedSiteIds => _assignedSiteIds.AsReadOnly();

    // Navigation to Sites
    private readonly List<Site> _assignedSites = new();
    public IReadOnlyCollection<Site> AssignedSites => _assignedSites.AsReadOnly();


    private User() : base() { }

    private User(
        string name,
        string email,
        string phoneNumber,
        UserRole role,
        Guid officeId) : base(Guid.NewGuid())
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
        OfficeId = officeId;
        IsActive = true;
        MustChangePassword = false;
    }

    public static User Create(
        string name,
        string email,
        string phoneNumber,
        UserRole role,
        Guid officeId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("User name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required");

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format");

        var user = new User(name, email, phoneNumber, role, officeId);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, name, email, role, officeId));
        return user;
    }

    public void SetPassword(string plainPassword, IPasswordHasher<User> hasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new DomainException("Password is required");

        if (hasher is null)
            throw new DomainException("Password hasher is required");

        PasswordHash = hasher.HashPassword(this, plainPassword);
    }

    public bool VerifyPassword(string plainPassword, IPasswordHasher<User> hasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || hasher is null || string.IsNullOrWhiteSpace(PasswordHash))
            return false;

        var result = hasher.VerifyHashedPassword(this, PasswordHash, plainPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    public void UpdateProfile(string name, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("User name is required");

        Name = name;
        PhoneNumber = phoneNumber;
        MarkAsUpdated(Email);
    }

    public void UpdateRole(UserRole newRole)
    {
        var oldRole = Role;
        Role = newRole;
        MarkAsUpdated(Email);
        
        if (oldRole != newRole)
        {
            AddDomainEvent(new UserRoleChangedEvent(Id, Name, oldRole, newRole, OfficeId));
        }
    }

    public void AssignToOffice(Guid officeId)
    {
        OfficeId = officeId;
        _assignedSiteIds.Clear(); // Clear site assignments when office changes
        MarkAsUpdated(Email);
    }

    public void SetEngineerCapacity(int maxSites, List<string> specializations)
    {
        if (Role != UserRole.PMEngineer)
            throw new DomainException("Only PM Engineers can have site capacity");

        if (maxSites <= 0)
            throw new DomainException("Max assigned sites must be greater than zero");

        MaxAssignedSites = maxSites;
        _specializations.Clear();
        if (specializations != null)
            _specializations.AddRange(specializations);
    }

    public void AssignSite(Guid siteId)
    {
        if (Role != UserRole.PMEngineer)
            throw new DomainException("Only PM Engineers can be assigned sites");

        if (MaxAssignedSites.HasValue && _assignedSiteIds.Count >= MaxAssignedSites.Value)
            throw new DomainException($"Engineer has reached maximum capacity of {MaxAssignedSites.Value} sites");

        if (!_assignedSiteIds.Contains(siteId))
        {
            _assignedSiteIds.Add(siteId);
        }
    }

    public void UnassignSite(Guid siteId)
    {
        _assignedSiteIds.Remove(siteId);
    }

    public void UpdatePerformanceRating(decimal rating)
    {
        if (rating < 0 || rating > 5)
            throw new DomainException("Performance rating must be between 0 and 5");

        PerformanceRating = rating;
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated(Email);
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated(Email);
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void RequirePasswordChange()
    {
        MustChangePassword = true;
        MarkAsUpdated(Email);
    }

    public void ClearPasswordChangeRequirement()
    {
        MustChangePassword = false;
        MarkAsUpdated(Email);
    }

    public bool CanBeAssignedMoreSites()
    {
        if (Role != UserRole.PMEngineer) return false;
        if (!MaxAssignedSites.HasValue) return true;
        return _assignedSiteIds.Count < MaxAssignedSites.Value;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
