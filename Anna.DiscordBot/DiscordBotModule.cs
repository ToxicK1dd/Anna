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
using Anna.DiscordBot.Config.Interface;
using Anna.DiscordBot.Config;
using Anna.DiscordBot.Helper.UnitOfWork.Interface;
using Anna.DiscordBot.Helper.UnitOfWork;
using System.Linq;
using System.Reflection;

namespace Anna.DiscordBot
{
    /// <summary>
    /// Module used for registering discord bot implementations.
    /// </summary>
    public class DiscordBotModule : Autofac.Module
    {
        /// <summary>
        /// Registers types related to the DiscordBot class library.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            RegisterHelpers(builder);

            builder.RegisterType<HelperUnitOfWork>()
                .As<IHelperUnitOfWork>()
                .InstancePerDependency()
                .OwnedByLifetimeScope();

            builder.RegisterType<Configurator>()
                .As<IConfigurator>()
                .SingleInstance();
        }


        #region Helpers
        private static void RegisterHelpers(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                 .Where(t => t.IsClass && t.Name.EndsWith("Helper"))
                 .As(t => t.GetInterfaces()
                 .Single(i => i.Name.EndsWith(t.Name)))
                .InstancePerDependency()
                .OwnedByLifetimeScope();
        } 
        #endregion
    }
}