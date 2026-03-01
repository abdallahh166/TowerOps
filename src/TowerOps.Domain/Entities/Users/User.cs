using Microsoft.AspNetCore.Identity;
using TowerOps.Domain.Common;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.UserEvents;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.Users;

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
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEndUtc { get; private set; }
    public int LockoutCountInWindow { get; private set; }
    public DateTime? LockoutWindowStartUtc { get; private set; }
    public bool IsManualLockout { get; private set; }
    public bool IsMfaEnabled { get; private set; }
    public string? MfaSecret { get; private set; }
    public string? ClientCode { get; private set; }
    public bool IsClientPortalUser { get; private set; }
    
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
            throw new DomainException("User name is required", "User.Name.Required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required", "User.Email.Required");

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format", "User.Email.InvalidFormat");

        var user = new User(name, email, phoneNumber, role, officeId);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, name, email, role, officeId));
        return user;
    }

    public void SetPassword(string plainPassword, IPasswordHasher<User> hasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new DomainException("Password is required", "User.Password.Required");

        if (hasher is null)
            throw new DomainException("Password hasher is required", "User.Password.HasherRequired");

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
            throw new DomainException("User name is required", "User.Name.Required");

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
            throw new DomainException("Only PM Engineers can have site capacity", "User.EngineerCapacity.RequiresPmEngineer");

        if (maxSites <= 0)
            throw new DomainException("Max assigned sites must be greater than zero", "User.EngineerCapacity.MaxAssignedSitesPositive");

        MaxAssignedSites = maxSites;
        _specializations.Clear();
        if (specializations != null)
            _specializations.AddRange(specializations);
    }

    public void AssignSite(Guid siteId)
    {
        if (Role != UserRole.PMEngineer)
            throw new DomainException("Only PM Engineers can be assigned sites", "User.AssignSite.RequiresPmEngineer");

        if (MaxAssignedSites.HasValue && _assignedSiteIds.Count >= MaxAssignedSites.Value)
            throw new DomainException(
                $"Engineer has reached maximum capacity of {MaxAssignedSites.Value} sites",
                "User.AssignSite.CapacityReached",
                MaxAssignedSites.Value);

        if (!_assignedSiteIds.Contains(siteId))
        {
            _assignedSiteIds.Add(siteId);
        }
    }

    public void UnassignSite(Guid siteId)
    {
        _assignedSiteIds.Remove(siteId);
    }

    public void ClearEngineerProfile()
    {
        _assignedSiteIds.Clear();
        _assignedSites.Clear();
        _specializations.Clear();
        MaxAssignedSites = null;
        PerformanceRating = null;
        MarkAsUpdated(Email);
    }

    public void UpdatePerformanceRating(decimal rating)
    {
        if (rating < 0 || rating > 5)
            throw new DomainException("Performance rating must be between 0 and 5", "User.PerformanceRating.Range");

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

    public bool IsLockedOut(DateTime nowUtc)
    {
        return IsManualLockout || (LockoutEndUtc.HasValue && LockoutEndUtc.Value > nowUtc);
    }

    public bool RegisterFailedLoginAttempt(
        DateTime nowUtc,
        int maxAttempts = 5,
        int temporaryLockoutMinutes = 15,
        int manualLockoutThreshold = 3,
        int manualWindowHours = 24)
    {
        if (IsManualLockout)
            return true;

        if (maxAttempts <= 0)
            maxAttempts = 5;

        if (temporaryLockoutMinutes <= 0)
            temporaryLockoutMinutes = 15;

        if (manualLockoutThreshold <= 0)
            manualLockoutThreshold = 3;

        if (manualWindowHours <= 0)
            manualWindowHours = 24;

        // Expired temporary lockout should not keep stale lock state.
        if (LockoutEndUtc.HasValue && LockoutEndUtc.Value <= nowUtc)
        {
            LockoutEndUtc = null;
        }

        FailedLoginAttempts++;
        if (FailedLoginAttempts < maxAttempts)
        {
            MarkAsUpdated(Email);
            return false;
        }

        FailedLoginAttempts = 0;
        var windowDuration = TimeSpan.FromHours(manualWindowHours);

        if (!LockoutWindowStartUtc.HasValue || (nowUtc - LockoutWindowStartUtc.Value) > windowDuration)
        {
            LockoutWindowStartUtc = nowUtc;
            LockoutCountInWindow = 1;
        }
        else
        {
            LockoutCountInWindow++;
        }

        if (LockoutCountInWindow >= manualLockoutThreshold)
        {
            IsManualLockout = true;
            LockoutEndUtc = null;
            MarkAsUpdated(Email);
            return true;
        }

        LockoutEndUtc = nowUtc.AddMinutes(temporaryLockoutMinutes);
        MarkAsUpdated(Email);
        return false;
    }

    public void RegisterSuccessfulLogin(DateTime nowUtc)
    {
        LastLoginAt = nowUtc;
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;

        // Keep manual lockout until explicit admin unlock.
        if (!IsManualLockout &&
            LockoutWindowStartUtc.HasValue &&
            (nowUtc - LockoutWindowStartUtc.Value) > TimeSpan.FromHours(24))
        {
            LockoutWindowStartUtc = null;
            LockoutCountInWindow = 0;
        }

        MarkAsUpdated(Email);
    }

    public void UnlockByAdmin()
    {
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
        LockoutCountInWindow = 0;
        LockoutWindowStartUtc = null;
        IsManualLockout = false;
        MarkAsUpdated(Email);
    }

    public void ConfigureMfa(string secret, bool enabled)
    {
        if (enabled && string.IsNullOrWhiteSpace(secret))
            throw new DomainException("MFA secret is required when enabling MFA.", "User.Mfa.SecretRequired");

        MfaSecret = string.IsNullOrWhiteSpace(secret) ? null : secret.Trim();
        IsMfaEnabled = enabled;
        MarkAsUpdated(Email);
    }

    public void EnableClientPortalAccess(string clientCode)
    {
        if (string.IsNullOrWhiteSpace(clientCode))
            throw new DomainException("Client code is required for portal access.", "User.ClientPortal.ClientCodeRequired");

        ClientCode = clientCode.Trim().ToUpperInvariant();
        IsClientPortalUser = true;
        MarkAsUpdated(Email);
    }

    public void DisableClientPortalAccess()
    {
        IsClientPortalUser = false;
        ClientCode = null;
        MarkAsUpdated(Email);
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
