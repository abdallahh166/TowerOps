using Microsoft.Extensions.Localization;
using System.Globalization;

namespace TelecomPm.Api.Localization;

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
        ["RequestFailed"] = "فشل تنفيذ الطلب",
        ["ValidationFailed"] = "فشل التحقق من صحة البيانات",
        ["InternalServerError"] = "حدث خطأ داخلي في الخادم",
        ["ForbiddenAction"] = "ليس لديك صلاحية لتنفيذ هذا الإجراء",
        ["InvalidCredentials"] = "بيانات الدخول غير صحيحة.",
        ["ForgotPasswordGenericSuccess"] = "إذا كان الحساب موجودًا، تم إرسال رمز تحقق إلى البريد الإلكتروني المسجل.",
        ["ResetPasswordSuccess"] = "تمت إعادة تعيين كلمة المرور بنجاح.",
        ["ChangePasswordSuccess"] = "تم تغيير كلمة المرور بنجاح.",
        ["Unauthorized"] = "غير مصرح.",
        ["VisitNotFound"] = "الزيارة غير موجودة.",
        ["SiteNotFound"] = "الموقع غير موجود.",
        ["WorkOrderNotFound"] = "أمر العمل غير موجود.",
        ["OfficeNotFound"] = "المكتب غير موجود.",
        ["UserNotFound"] = "المستخدم غير موجود.",
        ["MaterialNotFound"] = "المادة غير موجودة.",
        ["RoleNotFound"] = "الدور غير موجود.",
        ["SettingsUnsupportedService"] = "الخدمة غير مدعومة. استخدم twilio أو email أو firebase.",
        ["PortalAccessNotEnabled"] = "صلاحية البوابة غير مفعلة لهذا المستخدم.",
        ["OnlyAssignedEngineerCheckIn"] = "فقط المهندس المكلّف يمكنه تسجيل الوصول.",
        ["OnlyAssignedEngineerCheckOut"] = "فقط المهندس المكلّف يمكنه تسجيل المغادرة.",
        ["AuthenticatedUserRequired"] = "مطلوب مستخدم مصادق عليه.",
        ["AuthenticatedReviewerRequired"] = "مطلوب مراجع مصادق عليه.",
        ["FailedToOperationPrefix"] = "فشل تنفيذ العملية: {0}",
        ["EvidencePolicyValidationFailed"] = "فشل التحقق من سياسة الأدلة: {0}",
        ["VisitValidationFailed"] = "فشل التحقق من الزيارة: {0}",
        ["Validation.Required"] = "{0} مطلوب.",
        ["Validation.InvalidFormat"] = "تنسيق {0} غير صحيح.",
        ["Validation.MinLength"] = "يجب أن يكون {0} على الأقل {1} أحرف.",
        ["Validation.MaxLength"] = "يجب ألا يتجاوز {0} عدد {1} أحرف.",
        ["Field.Email"] = "البريد الإلكتروني",
        ["Field.Password"] = "كلمة المرور",
        ["Field.CurrentPassword"] = "كلمة المرور الحالية",
        ["Field.NewPassword"] = "كلمة المرور الجديدة",
        ["Field.ConfirmPassword"] = "تأكيد كلمة المرور",
        ["Field.PhoneNumber"] = "رقم الهاتف",
        ["Field.VisitId"] = "معرف الزيارة",
        ["Field.SiteId"] = "معرف الموقع",
        ["Field.SiteCode"] = "كود الموقع",
        ["Field.OfficeId"] = "معرف المكتب",
        ["Field.EngineerId"] = "معرف المهندس",
        ["Field.WorkOrderId"] = "معرف أمر العمل",
        ["Field.Id"] = "المعرف",
        ["Field.Name"] = "الاسم",
        ["Field.Role"] = "الدور"
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
