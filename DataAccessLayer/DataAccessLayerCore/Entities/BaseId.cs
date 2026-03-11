using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayerCore.Entities
{
    public abstract class BaseId
    {
        [Key]
        public long Id { get; set; }
        public required Guid Uuid { get; set; }
        public bool isActive = true;
        public DateTime CreatedDate = DateTime.Now;
    }
}
