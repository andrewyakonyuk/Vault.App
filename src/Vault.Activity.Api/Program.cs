﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Vault.Activity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //todo: temporary solution that fix scoped service resolution
                .UseDefaultServiceProvider(o => o.ValidateScopes = false)
                .Build();

            host.Run();
        }
    }
}
