using System;
using System.Collections.Generic;
using System.Text;

namespace StreamInsights.Abstractions
{
    public interface ICommittable
    {
        long CheckpointToken { get; set; }
    }
}
