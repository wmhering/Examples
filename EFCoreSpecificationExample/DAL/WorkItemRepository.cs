using EFCoreSpecificationExample.DAL.DbModel;
using EFCoreSpecificationExample.BLL;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EFCoreSpecificationExample.DAL
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly DataContext _dataContext;

        public WorkItemRepository(DataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public IImmutableList<WorkItemData> Search(bool currentOnly = false, int? olderThan = null, string? stateCode = null, DateTime? asOf = null)
        {
            var query = WorkItemQuery;
            if (currentOnly)
                query = query.Where(wi => wi.StateEndTime == new DateTime(9999, 12, 31, 23, 59, 59, 999));
            if (olderThan != null)
                query = query.Where(wi => wi.DaysOld > olderThan.Value);
            if (stateCode != null)
                query = query.Where(wi => wi.StateCode == stateCode);
            if (asOf != null)
                query = query.Where(wi => wi.StateEffectiveTime <= asOf && wi.StateEndTime > asOf);
            Console.WriteLine();
            Console.WriteLine(query.ToQueryString());
            return query.ToImmutableList();
        }

        public IImmutableList<WorkItemData> Search(Expression<Func<WorkItemData, bool>> predicate)
        {
            if (predicate == null)
                return Search((wi) => true);
            var query = WorkItemQuery.Where(predicate);
            Console.WriteLine();
            Console.WriteLine(query.ToQueryString());
            return query.ToImmutableList();
        }

        public IImmutableList<WorkItemData> Search(Specification<WorkItemData> specification)
        {
            var whereClause = specification?.ToExpression() ?? ((WorkItemData d) => true);
            var query = WorkItemQuery.Where(whereClause);
            Console.WriteLine();
            Console.WriteLine(query.ToQueryString());
            return query.ToImmutableList();
        }

        private IQueryable<WorkItemData> WorkItemQuery => _dataContext.Set<StateChange>()
            .Include(sc => sc.State)
            .Include(sc => sc.WorkItem)
            .Select(sc => new WorkItemData
            {
                Id = sc.WorkItem.WorkItemKey,
                Name = sc.WorkItem.Name,
                DateCreated = sc.WorkItem.DateCreated,
                DaysOld = sc.WorkItem.DaysOld,
                StateEffectiveTime = sc.EffectiveTime,
                StateEndTime = sc.EndTime,
                StateCode = sc.State.Code,
                StateName = sc.State.DisplayName
            });
    }
}
