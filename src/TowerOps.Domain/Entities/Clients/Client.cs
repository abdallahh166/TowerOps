using TowerOps.Domain.Common;
using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.Entities.Clients;

public sealed class Client : AggregateRoot<Guid>
{
    public string ClientCode { get; private set; } = string.Empty;
    public string ClientName { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }

    private Client() : base()
    {
    }

    private Client(
        string clientCode,
        string clientName,
        string? logoUrl,
        bool isActive,
        string? contactEmail,
        string? contactPhone) : base(Guid.NewGuid())
    {
        ClientCode = clientCode;
        ClientName = clientName;
        LogoUrl = logoUrl;
        IsActive = isActive;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
    }

    public static Client Create(
        string clientCode,
        string clientName,
        string? logoUrl = null,
        bool isActive = true,
        string? contactEmail = null,
        string? contactPhone = null)
    {
        if (string.IsNullOrWhiteSpace(clientCode))
            throw new DomainException("Client code is required.", "Client.Code.Required");

        if (string.IsNullOrWhiteSpace(clientName))
            throw new DomainException("Client name is required.", "Client.Name.Required");

        return new Client(
            clientCode.Trim().ToUpperInvariant(),
            clientName.Trim(),
            logoUrl,
            isActive,
            contactEmail,
            contactPhone);
    }

    public void Update(
        string clientName,
        string? logoUrl,
        bool isActive,
        string? contactEmail,
        string? contactPhone)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            throw new DomainException("Client name is required.", "Client.Name.Required");

        ClientName = clientName.Trim();
        LogoUrl = logoUrl;
        IsActive = isActive;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        MarkAsUpdated("System");
    }
}
