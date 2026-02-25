using Microsoft.Extensions.Localization;
using System.Globalization;

namespace TowerOps.Api.Localization;

public sealed class LocalizedTextService : ILocalizedTextService
{
    private readonly IStringLocalizer<ApiTextResources>? _localizer;

    private static readonly IReadOnlyDictionary<string, string> DefaultTexts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["RequestFailed"] = "Request failed",
        ["ValidationFailed"] = "Validation failed",
        ["InternalServerError"] = "An internal server error occurred",
        ["ForbiddenAction"] = "You do not have permission to perform this action",
        ["InvalidCredentials"] = "Invalid credentials.",
        ["ForgotPasswordGenericSuccess"] = "If the account exists, an OTP was sent to the registered email.",
        ["ResetPasswordSuccess"] = "Password has been reset successfully.",
        ["ChangePasswordSuccess"] = "Password changed successfully.",
        ["Unauthorized"] = "Unauthorized.",
        ["VisitNotFound"] = "Visit not found.",
        ["SiteNotFound"] = "Site not found.",
        ["WorkOrderNotFound"] = "Work order not found.",
        ["OfficeNotFound"] = "Office not found.",
        ["UserNotFound"] = "User not found.",
        ["MaterialNotFound"] = "Material not found.",
        ["RoleNotFound"] = "Role not found.",
        ["SettingsUnsupportedService"] = "Unsupported service. Use twilio, email, or firebase.",
        ["PortalAccessNotEnabled"] = "Portal access is not enabled for this user.",
        ["OnlyAssignedEngineerCheckIn"] = "Only the assigned engineer can check in.",
        ["OnlyAssignedEngineerCheckOut"] = "Only the assigned engineer can check out.",
        ["AuthenticatedUserRequired"] = "Authenticated user is required.",
        ["AuthenticatedReviewerRequired"] = "Authenticated reviewer is required",
        ["Error.EntityNotFound"] = "{0} with key '{1}' was not found",
        ["Error.BusinessRuleViolation"] = "Business rule '{0}' violated: {1}",
        ["Error.ConcurrencyConflict"] = "Concurrency conflict occurred for {0}",
        ["Error.UnauthorizedResourceAccess"] = "Unauthorized access to perform '{0}' on '{1}'",
        ["Error.InvalidStateTransition"] = "Cannot transition from '{0}' to '{1}'",
        ["Error.UnauthorizedGeneric"] = "You do not have permission to perform this action",
        ["Error.ConflictGeneric"] = "Request conflicts with current state"
    };

    private static readonly IReadOnlyDictionary<string, string> ArabicFallbackTexts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["RequestFailed"] = "ÙØ´Ù„ ØªÙ†ÙÙŠØ° Ø§Ù„Ø·Ù„Ø¨",
        ["ValidationFailed"] = "ÙØ´Ù„ Ø§Ù„ØªØ­Ù‚Ù‚ Ù…Ù† ØµØ­Ø© Ø§Ù„Ø¨ÙŠØ§Ù†Ø§Øª",
        ["InternalServerError"] = "Ø­Ø¯Ø« Ø®Ø·Ø£ Ø¯Ø§Ø®Ù„ÙŠ ÙÙŠ Ø§Ù„Ø®Ø§Ø¯Ù…",
        ["ForbiddenAction"] = "Ù„ÙŠØ³ Ù„Ø¯ÙŠÙƒ ØµÙ„Ø§Ø­ÙŠØ© Ù„ØªÙ†ÙÙŠØ° Ù‡Ø°Ø§ Ø§Ù„Ø¥Ø¬Ø±Ø§Ø¡",
        ["InvalidCredentials"] = "Ø¨ÙŠØ§Ù†Ø§Øª Ø§Ù„Ø¯Ø®ÙˆÙ„ ØºÙŠØ± ØµØ­ÙŠØ­Ø©.",
        ["ForgotPasswordGenericSuccess"] = "Ø¥Ø°Ø§ ÙƒØ§Ù† Ø§Ù„Ø­Ø³Ø§Ø¨ Ù…ÙˆØ¬ÙˆØ¯Ù‹Ø§ØŒ ØªÙ… Ø¥Ø±Ø³Ø§Ù„ Ø±Ù…Ø² ØªØ­Ù‚Ù‚ Ø¥Ù„Ù‰ Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ Ø§Ù„Ù…Ø³Ø¬Ù„.",
        ["ResetPasswordSuccess"] = "ØªÙ…Øª Ø¥Ø¹Ø§Ø¯Ø© ØªØ¹ÙŠÙŠÙ† ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ± Ø¨Ù†Ø¬Ø§Ø­.",
        ["ChangePasswordSuccess"] = "ØªÙ… ØªØºÙŠÙŠØ± ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ± Ø¨Ù†Ø¬Ø§Ø­.",
        ["Unauthorized"] = "ØºÙŠØ± Ù…ØµØ±Ø­.",
        ["VisitNotFound"] = "Ø§Ù„Ø²ÙŠØ§Ø±Ø© ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯Ø©.",
        ["SiteNotFound"] = "Ø§Ù„Ù…ÙˆÙ‚Ø¹ ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯.",
        ["WorkOrderNotFound"] = "Ø£Ù…Ø± Ø§Ù„Ø¹Ù…Ù„ ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯.",
        ["OfficeNotFound"] = "Ø§Ù„Ù…ÙƒØªØ¨ ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯.",
        ["UserNotFound"] = "Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù… ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯.",
        ["MaterialNotFound"] = "Ø§Ù„Ù…Ø§Ø¯Ø© ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯Ø©.",
        ["RoleNotFound"] = "Ø§Ù„Ø¯ÙˆØ± ØºÙŠØ± Ù…ÙˆØ¬ÙˆØ¯.",
        ["SettingsUnsupportedService"] = "Ø§Ù„Ø®Ø¯Ù…Ø© ØºÙŠØ± Ù…Ø¯Ø¹ÙˆÙ…Ø©. Ø§Ø³ØªØ®Ø¯Ù… twilio Ø£Ùˆ email Ø£Ùˆ firebase.",
        ["PortalAccessNotEnabled"] = "ØµÙ„Ø§Ø­ÙŠØ© Ø§Ù„Ø¨ÙˆØ§Ø¨Ø© ØºÙŠØ± Ù…ÙØ¹Ù„Ø© Ù„Ù‡Ø°Ø§ Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù….",
        ["OnlyAssignedEngineerCheckIn"] = "ÙÙ‚Ø· Ø§Ù„Ù…Ù‡Ù†Ø¯Ø³ Ø§Ù„Ù…ÙƒÙ„Ù‘Ù ÙŠÙ…ÙƒÙ†Ù‡ ØªØ³Ø¬ÙŠÙ„ Ø§Ù„ÙˆØµÙˆÙ„.",
        ["OnlyAssignedEngineerCheckOut"] = "ÙÙ‚Ø· Ø§Ù„Ù…Ù‡Ù†Ø¯Ø³ Ø§Ù„Ù…ÙƒÙ„Ù‘Ù ÙŠÙ…ÙƒÙ†Ù‡ ØªØ³Ø¬ÙŠÙ„ Ø§Ù„Ù…ØºØ§Ø¯Ø±Ø©.",
        ["AuthenticatedUserRequired"] = "Ù…Ø·Ù„ÙˆØ¨ Ù…Ø³ØªØ®Ø¯Ù… Ù…ØµØ§Ø¯Ù‚ Ø¹Ù„ÙŠÙ‡.",
        ["AuthenticatedReviewerRequired"] = "Ù…Ø·Ù„ÙˆØ¨ Ù…Ø±Ø§Ø¬Ø¹ Ù…ØµØ§Ø¯Ù‚ Ø¹Ù„ÙŠÙ‡.",
        ["FailedToOperationPrefix"] = "ÙØ´Ù„ ØªÙ†ÙÙŠØ° Ø§Ù„Ø¹Ù…Ù„ÙŠØ©: {0}",
        ["EvidencePolicyValidationFailed"] = "ÙØ´Ù„ Ø§Ù„ØªØ­Ù‚Ù‚ Ù…Ù† Ø³ÙŠØ§Ø³Ø© Ø§Ù„Ø£Ø¯Ù„Ø©: {0}",
        ["VisitValidationFailed"] = "ÙØ´Ù„ Ø§Ù„ØªØ­Ù‚Ù‚ Ù…Ù† Ø§Ù„Ø²ÙŠØ§Ø±Ø©: {0}",
        ["Validation.Required"] = "{0} Ù…Ø·Ù„ÙˆØ¨.",
        ["Validation.InvalidFormat"] = "ØªÙ†Ø³ÙŠÙ‚ {0} ØºÙŠØ± ØµØ­ÙŠØ­.",
        ["Validation.MinLength"] = "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† {0} Ø¹Ù„Ù‰ Ø§Ù„Ø£Ù‚Ù„ {1} Ø£Ø­Ø±Ù.",
        ["Validation.MaxLength"] = "ÙŠØ¬Ø¨ Ø£Ù„Ø§ ÙŠØªØ¬Ø§ÙˆØ² {0} Ø¹Ø¯Ø¯ {1} Ø£Ø­Ø±Ù.",
        ["Field.Email"] = "Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ",
        ["Field.Password"] = "ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ±",
        ["Field.CurrentPassword"] = "ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ± Ø§Ù„Ø­Ø§Ù„ÙŠØ©",
        ["Field.NewPassword"] = "ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ± Ø§Ù„Ø¬Ø¯ÙŠØ¯Ø©",
        ["Field.ConfirmPassword"] = "ØªØ£ÙƒÙŠØ¯ ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ±",
        ["Field.PhoneNumber"] = "Ø±Ù‚Ù… Ø§Ù„Ù‡Ø§ØªÙ",
        ["Field.VisitId"] = "Ù…Ø¹Ø±Ù Ø§Ù„Ø²ÙŠØ§Ø±Ø©",
        ["Field.SiteId"] = "Ù…Ø¹Ø±Ù Ø§Ù„Ù…ÙˆÙ‚Ø¹",
        ["Field.SiteCode"] = "ÙƒÙˆØ¯ Ø§Ù„Ù…ÙˆÙ‚Ø¹",
        ["Field.OfficeId"] = "Ù…Ø¹Ø±Ù Ø§Ù„Ù…ÙƒØªØ¨",
        ["Field.EngineerId"] = "Ù…Ø¹Ø±Ù Ø§Ù„Ù…Ù‡Ù†Ø¯Ø³",
        ["Field.WorkOrderId"] = "Ù…Ø¹Ø±Ù Ø£Ù…Ø± Ø§Ù„Ø¹Ù…Ù„",
        ["Field.Id"] = "Ø§Ù„Ù…Ø¹Ø±Ù",
        ["Field.Name"] = "Ø§Ù„Ø§Ø³Ù…",
        ["Field.Role"] = "Ø§Ù„Ø¯ÙˆØ±"
    };

    private static readonly IReadOnlyDictionary<string, string> MessageToKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Invalid credentials."] = "InvalidCredentials",
        ["Unauthorized."] = "Unauthorized",
        ["Visit not found"] = "VisitNotFound",
        ["Visit not found."] = "VisitNotFound",
        ["Site not found"] = "SiteNotFound",
        ["Site not found."] = "SiteNotFound",
        ["Work order not found"] = "WorkOrderNotFound",
        ["Work order not found."] = "WorkOrderNotFound",
        ["Office not found"] = "OfficeNotFound",
        ["Office not found."] = "OfficeNotFound",
        ["User not found"] = "UserNotFound",
        ["User not found."] = "UserNotFound",
        ["Material not found"] = "MaterialNotFound",
        ["Material not found."] = "MaterialNotFound",
        ["Role not found."] = "RoleNotFound",
        ["Portal access is not enabled for this user."] = "PortalAccessNotEnabled",
        ["Only the assigned engineer can check in."] = "OnlyAssignedEngineerCheckIn",
        ["Only the assigned engineer can check out."] = "OnlyAssignedEngineerCheckOut",
        ["Authenticated user is required."] = "AuthenticatedUserRequired",
        ["Authenticated reviewer is required"] = "AuthenticatedReviewerRequired"
    };

    // Parameterless constructor keeps test/controller fallback behavior when DI is not fully composed.
    public LocalizedTextService()
    {
    }

    public LocalizedTextService(IStringLocalizer<ApiTextResources> localizer)
    {
        _localizer = localizer;
    }

    public string Get(string key, string? fallback = null, params object[] formatArgs)
    {
        string value;

        if (_localizer is not null)
        {
            var localized = _localizer[key];
            if (!localized.ResourceNotFound)
            {
                value = localized.Value;
                return ApplyFormat(value, formatArgs);
            }
        }

        if (IsArabicCulture() && ArabicFallbackTexts.TryGetValue(key, out var arabic))
        {
            value = arabic;
            return ApplyFormat(value, formatArgs);
        }

        if (DefaultTexts.TryGetValue(key, out var defaultText))
        {
            value = defaultText;
            return ApplyFormat(value, formatArgs);
        }

        value = fallback ?? key;
        return ApplyFormat(value, formatArgs);
    }

    public string TranslateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return message;

        if (MessageToKey.TryGetValue(message, out var key))
            return Get(key, message);

        if (message.StartsWith("Failed to ", StringComparison.OrdinalIgnoreCase))
        {
            return string.Format(
                Get("FailedToOperationPrefix", "Failed to process operation: {0}"),
                message);
        }

        if (message.StartsWith("Evidence policy validation failed:", StringComparison.OrdinalIgnoreCase))
        {
            var suffix = message["Evidence policy validation failed:".Length..].Trim();
            return string.Format(
                Get("EvidencePolicyValidationFailed", "Evidence policy validation failed: {0}"),
                suffix);
        }

        if (message.StartsWith("Visit validation failed:", StringComparison.OrdinalIgnoreCase))
        {
            var suffix = message["Visit validation failed:".Length..].Trim();
            return string.Format(
                Get("VisitValidationFailed", "Visit validation failed: {0}"),
                suffix);
        }

        return message;
    }

    private static bool IsArabicCulture()
    {
        return string.Equals(
            System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
            "ar",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string ApplyFormat(string value, object[]? formatArgs)
    {
        if (formatArgs is null || formatArgs.Length == 0)
            return value;

        return string.Format(CultureInfo.CurrentCulture, value, formatArgs);
    }
}
