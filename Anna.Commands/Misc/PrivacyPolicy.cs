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
using DSharpPlus.Entities;
using Anna.Commands.Base;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Misc
{
    public class Privacy : CommandBase
    {
        [Command("privacy")]
        [Description("Privacy Policy required by Discord")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public virtual async Task GetPrivacyPolicyAsync(CommandContext ctx)
        {
            try
            {
                // Trigger Typing
                await ctx.TriggerTypingAsync();

                // Create the embed
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                {
                    Title = "Fortrolighedspolitik",
                    Color = new DiscordColor(104, 194, 175),
                    Description = "Anna's fortrolighedspolitik, et krav fra discord at have fra den 18. august."
                };
                // Add fields
                embedBuilder.AddField("Brug af Server ID'er:", "Anna gemmer din servers ID, hvis du aktivere en feature eller ændre din prefix! Dette gør at vi kan finde din server og sørge for at du får det, som du har aktiveret/ændret.", false);

                embedBuilder.AddField("Brug af Bruger ID'er:", "Anna gemmer bruger ID'er, hvis du f.eks. laver en konto. Ændre du noget som kun ændres for din bruger, så kan du være meget sikker på at vi har gemt dit ID, for at du har adgang til det du har ændret eller aktiveret.", false);

                embedBuilder.AddField("Fjernelse af Data:", "Hvis du vil have data fjernet, så kontakt os på vores [support server](https://discord.gg/ks3n5hK), hvis du kicker Anna vil alle data om din server blive fjernet, __ikke__ dine bruger data.", false);

                embedBuilder.AddField("Andet?", "Vi gemmer ikke logs, beskeder, profilbilleder eller ligende. Skriv til os på vores [support server](https://discord.gg/ks3n5hK) hvis du har spørgsmål.", false);

                // Send embedded message
                await ctx.RespondAsync(embedBuilder);
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