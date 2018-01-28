using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamInsights.Json
{
    public interface IJTokenVisitor<in TContext>
    {
        void Visit(JToken json, TContext context);
    }
}
