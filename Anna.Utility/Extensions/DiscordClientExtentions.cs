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
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Anna.Utility.Extensions
{
    public static class DiscordClientExtentions
    {
        /// <summary>
        /// <para>Updates the activity on a <see cref="DiscordClient"/> object.</para>
        /// <para>If the <see cref="ActivityType"/> is <see cref="ActivityType.Streaming"/>, you have to remember to set the optional streamUrl parameter.</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="status"></param>
        /// <param name="activityType"></param>
        /// <param name="streamUrl"></param>
        /// <returns></returns>
        public static async Task UpdateActivityAsync(this DiscordClient client, string status, ActivityType activityType, string streamUrl = null)
        {
            // Create new activity object
            DiscordActivity activity = new()
            {
                Name = status,
                ActivityType = activityType,
                StreamUrl = streamUrl
            };

            // Update activity
            await client.UpdateStatusAsync(activity);
        }
    }
}