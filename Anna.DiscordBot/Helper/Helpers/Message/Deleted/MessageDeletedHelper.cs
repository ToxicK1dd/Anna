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

using DSharpPlus.EventArgs;
using DSharpPlus;
using Anna.DiscordBot.Helper.Helpers.Message.Deleted.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Anna.DiscordBot.Helper.Message.Deleted
{
    public class MessageDeletedHelper : IMessageDeletedHelper
    {
        public virtual async Task OnMessageDeleted(DiscordClient client, MessageDeleteEventArgs e)
        {
            try
            {
                // Put logic here
                await Task.Delay(1);
            }
            catch(Exception ex)
            {
                // Log error message
                client.Logger.Log(LogLevel.Error, new EventId(101, "MessageCreated"), ex.Message ?? ex.InnerException.Message);
            }
        }
    }
}