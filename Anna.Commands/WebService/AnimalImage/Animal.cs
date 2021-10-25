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
using Anna.WebService.WebServices.Derived.AnimalImage.Interface;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.WebService.AnimalImage
{
    /// <summary>
    /// Get a random image of an animal.
    /// </summary>
    public class Animal : CommandBase
    {
        private readonly IAnimalImageService service;

        public Animal(IAnimalImageService service)
        {
            this.service = service;
        }


        [Hidden()]
        [Command("animal")]
        [Description("Returns a random animal image from the internet!")]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public virtual async Task GetRandomAnimalAsync(CommandContext ctx)
        {
            // Get random number
            int randomNumber = new Random().Next(1, 11);

            switch(randomNumber)
            {
                case 1:
                    await new Birb(service).GetBirbImageAsync(ctx);
                    break;
                case 2:
                    await new Cat(service).GetCatImageAsync(ctx);
                    break;
                case 3:
                    await new Dog(service).GetDogImageAsync(ctx);
                    break;
                case 4:
                    await new Fox(service).GetFoxImageAsync(ctx);
                    break;
                case 5:
                    await new Kangaroo(service).GetKangarooImageAsync(ctx);
                    break;
                case 6:
                    await new Koala(service).GetKoalaImageAsync(ctx);
                    break;
                case 7:
                    await new Panda(service).GetPandaImageAsync(ctx);
                    break;
                case 8:
                    await new Racoon(service).GetRacoonImageAsync(ctx);
                    break;
                case 9:
                    await new RedPanda(service).GetRedPandaImageAsync(ctx);
                    break;
                case 10:
                    await new Whale(service).GetWhaleImageAsync(ctx);
                    break;
            }
        }
    }
}