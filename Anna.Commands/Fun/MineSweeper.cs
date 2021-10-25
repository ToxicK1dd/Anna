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

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using Anna.Commands.Base;
using Anna.Model.MineField;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Fun
{
    public class MineSweeper : CommandBase
    {
        [Hidden()]
        [Command("mine")]
        [Aliases("minesweeper", "sweeper")]
        [Description("Tag et spil minesweeper!")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public virtual async Task GetMineSweeperAsync(CommandContext ctx)
        {
            try
            {
                // Trigger typing
                await ctx.TriggerTypingAsync();

                // Create minefield
                Minesweeper field = new(rows: 9, columns: 9);

                // Respond with created minefield
                await ctx.RespondAsync(field.Create());
            }
            catch(Exception ex)
            {
                // Handle exception.
                await HandleExceptionAsync(ctx, ex);

                // Rethrow for use elsewhere.
                throw;
            }
        }
    }
}