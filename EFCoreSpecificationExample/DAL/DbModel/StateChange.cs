using System.ComponentModel.DataAnnotations;

namespace EFCoreSpecificationExample.DAL.DbModel
{
    public class StateChange
    {
        [Required]
        public int? StateChangeKey { get; set; }

        [Required]
        public int? StateKey { get; set; }

        [Required]
        public int? WorkItemKey { get; set; }

        [Required]
        public DateTime? EffectiveTime { get; set; }

        public DateTime? EndTime { get; set; }


        public State State { get; set; }

        public WorkItem WorkItem { get; set; }
    }
}
