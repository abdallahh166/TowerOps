using TowerOps.Domain.Entities.SystemSettings;

namespace TowerOps.Domain.Specifications.SystemSettingSpecifications;

public sealed class AllSystemSettingsSpecification : BaseSpecification<SystemSetting>
{
    public AllSystemSettingsSpecification(
        int skip,
        int take,
        string sortBy = "group",
        bool sortDescending = false)
        : base(s => !s.IsDeleted)
    {
        switch (sortBy.Trim().ToLowerInvariant())
        {
            case "key":
                if (sortDescending) ApplyOrderByDescending(s => s.Key);
                else ApplyOrderBy(s => s.Key);
                break;
            case "datatype":
                if (sortDescending) ApplyOrderByDescending(s => s.DataType);
                else ApplyOrderBy(s => s.DataType);
                break;
            case "updatedatutc":
                if (sortDescending) ApplyOrderByDescending(s => s.UpdatedAtUtc);
                else ApplyOrderBy(s => s.UpdatedAtUtc);
                break;
            case "group":
            default:
                if (sortDescending) ApplyOrderByDescending(s => s.Group);
                else ApplyOrderBy(s => s.Group);
                AddThenBy(s => s.Key);
                break;
        }

        ApplyPaging(skip, take);
    }
}
