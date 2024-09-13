using System.Linq.Expressions;

namespace EFCoreSpecificationExample.BLL;

public record WorkItemData
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public DateTime? DateCreated { get; set; }
    public int? DaysOld { get; set; }
    public DateTime? StateEffectiveTime { get; set; }
    public DateTime? StateEndTime { get; set; }
    public string StateCode {  get; set; }
    public string StateName { get; set; }

    public class CurrentStateSpecification : Specification<WorkItemData>
    {
        public override Expression<Func<WorkItemData, bool>> ToExpression() =>
            (workItem) => workItem.StateEndTime == new DateTime(9999, 12, 31, 23, 59, 59, 999);
    }

    public class OlderThanSpecification : Specification<WorkItemData>
    {
        private readonly int _days;

        public OlderThanSpecification(int days)
        {
            if (days <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(days)} must be positive integer");
            _days = days;
        }

        public override Expression<Func<WorkItemData, bool>> ToExpression() =>
            (workItem) => (workItem.DaysOld ?? 0) > _days;
    }

    public class StateIsSpecification : Specification<WorkItemData>
    {
        private string _code;

        public StateIsSpecification(string code)
        {
            _code = !string.IsNullOrWhiteSpace(code) ? code : throw new ArgumentException($"{nameof(code)} is required");
        }

        public override Expression<Func<WorkItemData, bool>> ToExpression() =>
            (workItem) => workItem.StateCode == _code;
    }

    public class AsOfSpecification : Specification<WorkItemData>
    {
        DateTime _time;

        public AsOfSpecification(DateTime time)
        {
            _time = time;
        }

        public override Expression<Func<WorkItemData, bool>> ToExpression() =>
            (workItem) => workItem.StateEffectiveTime <= _time && workItem.StateEndTime > _time;
    }
}
