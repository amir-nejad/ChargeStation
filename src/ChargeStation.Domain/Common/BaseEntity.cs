using System;
using System.Collections.Generic;

namespace ChargeStation.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

        public DateTime LastModifiedDateUtc { get; set; } = DateTime.UtcNow;
    }
}
