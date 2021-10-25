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

using Anna.DiscordBot.Helper.Helpers.Member.Added.Interface;
using Anna.DiscordBot.Helper.Helpers.Member.Removed.Interface;
using Anna.DiscordBot.Helper.Helpers.Member.Update.Interface;
using Anna.DiscordBot.Helper.Helpers.Message.Created.Interface;
using Anna.DiscordBot.Helper.Helpers.Message.Deleted.Interface;
using Anna.DiscordBot.Helper.Helpers.Message.Updated.Interface;
using Anna.DiscordBot.Helper.UnitOfWork.Interface;

namespace Anna.DiscordBot.Helper.UnitOfWork
{
    public class HelperUnitOfWork : IHelperUnitOfWork
    {
        public HelperUnitOfWork(
            // Member
            IMemberAddedHelper memberAddedHelper,
            IMemberRemovedHelper memberRemovedHelper,
            IMemberUpdatedHelper memberUpdatedHelper,
            // Message
            IMessageCreatedHelper messageCreatedHelper,
            IMessageDeletedHelper messageDeletedHelper,
            IMessageUpdatedHelper messageUpdatedHelper)
        {
            // Member
            GuildMemberAddedHelper = memberAddedHelper;
            GuildMemberRemovedHelper = memberRemovedHelper;
            GuildMemberUpdatedHelper = memberUpdatedHelper;

            // Message 
            MessageCreatedHelper = messageCreatedHelper;
            MessageDeletedHelper = messageDeletedHelper;
            MessageUpdatedHelper = messageUpdatedHelper;
        }

        // Member
        public virtual IMemberAddedHelper GuildMemberAddedHelper { get; }
        public virtual IMemberRemovedHelper GuildMemberRemovedHelper { get; }
        public virtual IMemberUpdatedHelper GuildMemberUpdatedHelper { get; }

        // Message
        public virtual IMessageCreatedHelper MessageCreatedHelper { get; }
        public virtual IMessageDeletedHelper MessageDeletedHelper { get; }
        public virtual IMessageUpdatedHelper MessageUpdatedHelper { get; }
    }
}