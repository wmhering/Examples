using System.ComponentModel.DataAnnotations;

namespace EFCoreSpecificationExample.DAL.DbModel;

public class State
{
    [Required]
    public int? StateKey {  get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string DisplayName { get; set; }

    public ICollection<StateChange> StateChanges { get; set; }
}
