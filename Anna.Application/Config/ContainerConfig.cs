// Copyright (c) 2021 ToxicK1dd
// Copyright (C) 2021 Project Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Autofac.Extensions.DependencyInjection;
using Autofac;
using Anna.DataAccess;
using Anna.DiscordBot;
using Anna.Model.Singleton;
using Anna.Service;
using Anna.WebService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;

namespace Anna.Application.Config
{
    /// <summary>
    /// General configuration of the Autofac IoC container.
    /// </summary>
    public static class ContainerConfig
    {
        /// <summary>
        /// Registers all application modules.
        /// </summary>
        public static ContainerBuilder Configure(this ContainerBuilder builder)
        {
            // This configures the IServiceProvider, and other related services.
            IConfigurationRoot configuration = builder.ConfigureServices();

            builder.RegisterModule(new DataAccessModule(configuration));

            builder.RegisterModule(new DiscordBotModule());

            builder.RegisterModule(new ServiceModule());

            builder.RegisterModule(new WebServiceModule());

            // Return the built container
            return builder;
        }


        #region Helpers
        /// <summary>
        /// Loads miscellaneous items, and makes IServiceProvider available for DSharpPlus
        /// </summary>
        /// <param name="builder"></param>
        private static IConfigurationRoot ConfigureServices(this ContainerBuilder builder)
        {
            // New service collection needed for injection into DSharpPlus, and other stuff
            ServiceCollection services = new();

            // Add memory cache for caching different things around the application
            services.AddMemoryCache(
                option => option.ExpirationScanFrequency = TimeSpan.FromSeconds(30));

            // Add options for our appsettings configuration
            services.AddOptions();

            // Load appsettings.json for getting section values
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            // Add channels section values to logging channel type
            services.Configure<LoggingChannels>(configuration.GetSection("Channels"));

            // Add configuration as a singleton
            services.AddSingleton(configuration);

            // Make IServiceProvider accessible in our IoC container
            builder.Populate(services);

            // Return configuration
            return configuration;
        }
        #endregion
    }
}