﻿using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Каталог_фильмов.Areas.Identity.IdentityHostingStartup))]
namespace Каталог_фильмов.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}