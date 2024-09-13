using System.ComponentModel.DataAnnotations;

namespace EFCoreSpecificationExample.DAL.DbModel
{
    public class WorkItem
    {
        [Required]
        public int? WorkItemKey { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime? DateCreated { get; set; }

        [Required]
        public int? DaysOld { get; set; }

        public ICollection<StateChange> StateChanges { get; set; }
    }
}
