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

using Anna.DataAccess.Models.Watchdog;
using Anna.DataAccess.Repository.Base;
using Anna.DataAccess.Repository.Derived.Watchdog.Guild.Interface;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Anna.DataAccess.Repository.Derived.Watchdog.Guild
{
    public class GuildRepository : RepositoryBase<Models.Watchdog.Guild.Guild, WatchdogContext>, IGuildRepository
    {
        public GuildRepository(WatchdogContext context) : base(context) { }


        public async Task<Models.Watchdog.Guild.Guild> GetByIdWithReportsAsync(ulong id)
        {
            return await context.Set<Models.Watchdog.Guild.Guild>()
                .Include(x => x.Reports)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.Watchdog.Guild.Guild> GetByIdWithFileRestrictionsAsync(ulong id)
        {
            return await context.Set<Models.Watchdog.Guild.Guild>()
                .Include(x => x.FileRestrictions)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.Watchdog.Guild.Guild> GetByIdWithFileRestrictionsAndBannedLinksAsync(ulong id)
        {
            return await context.Set<Models.Watchdog.Guild.Guild>()
                .Include(x => x.FileRestrictions)
                .AsSplitQuery()
                .Include(x => x.BannedLinks)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}