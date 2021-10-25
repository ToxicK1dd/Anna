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

using Autofac;
using Anna.DataAccess.Models.Watchdog;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;

namespace Anna.DataAccess
{
    /// <summary>
    /// Module used for registering data access implementations.
    /// </summary>
    public class DataAccessModule : Autofac.Module
    {
        private readonly IConfiguration configuration;

        public DataAccessModule(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        /// <summary>
        /// Registers types related to the DataAccess class library.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            RegisterWatchdogContexts(builder);

            RegisterRepositories(builder);

            RegisterUnitOfWorks(builder);
        }


        #region Helpers
        private void RegisterWatchdogContexts(ContainerBuilder builder)
        {
            builder.RegisterType<WatchdogContext>()
                .WithParameter(
                    "options",
                    WatchdogOptionsFactory.Get(configuration.GetConnectionString("Watchdog")))
                .InstancePerDependency()
                .OwnedByLifetimeScope();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsClass && t.Name.EndsWith("Repository"))
                .As(t => t.GetInterfaces()
                .Single(i => i.Name.EndsWith(t.Name)))
                .InstancePerDependency()
                .OwnedByLifetimeScope();
        }

        private static void RegisterUnitOfWorks(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsClass && t.Name.EndsWith("UnitOfWork"))
                .As(t => t.GetInterfaces()
                .Single(i => i.Name.EndsWith(t.Name)))
                .InstancePerDependency()
                .OwnedByLifetimeScope();
        }
        #endregion
    }
}
