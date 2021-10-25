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
using Anna.DataAccess.Repository.Derived.Watchdog.FileRestriction.Interface;
using Anna.DataAccess.Repository.Derived.Watchdog.FileRestriction;
using Anna.DataAccess.Repository.Derived.Watchdog.Guild.Interface;
using Anna.DataAccess.Repository.Derived.Watchdog.Guild;
using Anna.DataAccess.Repository.Derived.Watchdog.LinkBan.Interface;
using Anna.DataAccess.Repository.Derived.Watchdog.LinkBan;
using Anna.DataAccess.Repository.Derived.Watchdog.Report.Interface;
using Anna.DataAccess.Repository.Derived.Watchdog.Report;
using Anna.DataAccess.UnitOfWork.Base;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;

namespace Anna.DataAccess.UnitOfWork.Derived.Watchdog
{
    public class WatchdogUnitOfWork : UnitOfWorkBase<WatchdogContext>, IWatchdogUnitOfWork
    {
        private GuildRepository guildRepository;
        private ReportRepository reportRepository;
        private FileRestrictionRepository fileRestrictionRepository;
        private LinkBanRepository linkBanRepository;

        public WatchdogUnitOfWork(WatchdogContext context) : base(context) { }


        public IGuildRepository GuildRepository { get => guildRepository ??= new(context); }
        public IReportRepository ReportRepository { get => reportRepository ??= new(context); }
        public IFileRestrictionRepository FileRestrictionRepository { get => fileRestrictionRepository ??= new(context); }
        public ILinkBanRepository LinkBanRepository { get => linkBanRepository ??= new(context); }
    }
}