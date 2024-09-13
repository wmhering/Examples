using System.Collections.Immutable;
using System.Linq.Expressions;

namespace EFCoreSpecificationExample.BLL
{
    public interface IWorkItemRepository
    {
        IImmutableList<WorkItemData> Search(bool currentOnly = false, int? olderThan = null, string? stateCode = null, DateTime? asOf = null);

        IImmutableList<WorkItemData> Search(Expression<Func<WorkItemData, bool>> predicate);

        IImmutableList<WorkItemData> Search(Specification<WorkItemData> specification);
    }
}
