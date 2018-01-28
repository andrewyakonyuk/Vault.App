using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public interface IActivityClient
    {
        IActivityStream GetStream(string bucket, string streamId);
    }
}
