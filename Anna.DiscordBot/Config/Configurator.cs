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

using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus;
using Anna.DiscordBot.Config.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace Anna.DiscordBot.Config
{
    public class Configurator : IConfigurator
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public Configurator(IServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }


        public async Task<DiscordShardedClient> ConfigureAsync()
        {
            // Initialize a new client instance
            DiscordShardedClient client = new(new()
            {
                Token = configuration.GetSection("Config:Token").Value,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All,
            });

            // Setup CommandsNext configuration.
            IReadOnlyDictionary<int, CommandsNextExtension> commands = await client.UseCommandsNextAsync(new()
            {
                Services = serviceProvider,
                StringPrefixes = new string[] { configuration.GetSection("Config:Prefix").Value },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false,
                EnableDefaultHelp = false,
                CaseSensitive = false
            });

            // Register commands, and command error handler
            commands.RegisterCommands(Assembly.Load("Anna.Commands"));
            foreach (var item in commands)
                item.Value.CommandErrored += CommandErrored;

            // Setup Interactivity configuration
            await client.UseInteractivityAsync(new()
            {
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
                PaginationDeletion = DSharpPlus.Interactivity.Enums.PaginationDeletion.KeepEmojis,
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
            });

            // Return client
            return client;
        }


        #region CommandErrored
        private Task CommandErrored(CommandsNextExtension extension, CommandErrorEventArgs e)
        {
            try
            {
                if (e.Command != null)
                {
                    // Log error message.
                    extension.Client.Logger.Log(
                        LogLevel.Error,
                        new EventId(101, "CmdError"),
                        $"{e.Context.Member.DisplayName}#{e.Context.Member.Discriminator}/{e.Context.Channel.Id}/{e.Command.Name}");

                    return Task.CompletedTask;
                }

                // Log error message.
                extension.Client.Logger.Log(
                    LogLevel.Error,
                    new EventId(101, "CmdError"),
                    $"{e.Context.Member.DisplayName}#{e.Context.Member.Discriminator}/{e.Context.Channel.Id}/{e.Context.Message.Content}");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Log error message.
                extension.Client.Logger.Log(LogLevel.Error, new EventId(101, "CmdError"), ex.Message);

                return Task.CompletedTask;
            }
        }
        #endregion
    }
}