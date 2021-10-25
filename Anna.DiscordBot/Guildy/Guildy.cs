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

using DSharpPlus;
using Anna.DiscordBot.Config.Interface;
using Anna.DiscordBot.Helper.UnitOfWork.Interface;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Anna.DiscordBot.Anna
{
    public class Anna : IHostedService
    {
        public Anna(IConfigurator configurator, IHelperUnitOfWork helperUnitOfWork)
        {
            Configurator = configurator;
            HelperUnitOfWork = helperUnitOfWork;
        }


        public IConfigurator Configurator { get; }

        public IHelperUnitOfWork HelperUnitOfWork { get; }

        public DiscordShardedClient Client { get; set; }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Configure the client
            Client = await Configurator.ConfigureAsync();

            // Member helpers
            Client.GuildMemberAdded += HelperUnitOfWork.GuildMemberAddedHelper.OnMemberAddedAsync;
            Client.GuildMemberRemoved += HelperUnitOfWork.GuildMemberRemovedHelper.OnMemberRemovedAsync;
            Client.GuildMemberUpdated += HelperUnitOfWork.GuildMemberUpdatedHelper.OnMemberUpdatedAsync;

            // Message helpers
            Client.MessageCreated += HelperUnitOfWork.MessageCreatedHelper.OnMessageCreated;
            Client.MessageDeleted += HelperUnitOfWork.MessageDeletedHelper.OnMessageDeleted;
            Client.MessageUpdated += HelperUnitOfWork.MessageUpdatedHelper.OnMessageUpdated;

            // Connect bot to Discord
            await Client.StartAsync();

            // Keep this task running forever, thereby keeping the bot running
            await Task.Delay(-1, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await Client.StopAsync();
    }
}