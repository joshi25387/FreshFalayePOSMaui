using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalProductGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ProductGroupId { get; set; }   // SAME as server ID
        public Guid SyncId { get; set; }   // SAME as server ID

        public string GroupName { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
