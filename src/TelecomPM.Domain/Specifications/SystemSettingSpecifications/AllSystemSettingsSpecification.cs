using TelecomPM.Domain.Entities.SystemSettings;

namespace TelecomPM.Domain.Specifications.SystemSettingSpecifications;

public sealed class AllSystemSettingsSpecification : BaseSpecification<SystemSetting>
{
    public AllSystemSettingsSpecification(int skip, int take)
        : base(s => !s.IsDeleted)
    {
        ApplyOrderBy(s => s.Group);
        AddThenBy(s => s.Key);
        ApplyPaging(skip, take);
    }
}
