namespace NizamDesk.Entities;

public record ExternalUserInfo(
    string Provider,
    string ProviderId,
    string? Name,
    string? Email,
    string? AccessToken
);