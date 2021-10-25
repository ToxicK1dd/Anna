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

using Anna.Utility.Enums;
using Anna.WebService.WebServices.Base;
using Anna.WebService.WebServices.Derived.AnimalImage.Interface;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Anna.WebService.WebServices.Derived.AnimalImage
{
    public class AnimalImageService : WebServiceBase, IAnimalImageService
    {
        public virtual async Task<string> GetAnimalImageAsync(AnimalType image)
        {
            // Empty string for storing the json result
            string json = string.Empty;

            // ifififififififififi
            switch(image)
            {
                case AnimalType.Birb:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/birb");
                    break;
                case AnimalType.Cat:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/cat");
                    break;
                case AnimalType.Dog:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/dog");
                    break;
                case AnimalType.Fox:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/fox");
                    break;
                case AnimalType.Kangaroo:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/kangaroo");
                    break;
                case AnimalType.Koala:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/koala");
                    break;
                case AnimalType.Panda:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/panda");
                    break;
                case AnimalType.Pikachu:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/pikachu");
                    break;
                case AnimalType.Racoon:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/racoon");
                    break;
                case AnimalType.RedPanda:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/red_panda");
                    break;
                case AnimalType.Whale:
                    json = await CallWebApiAsync("https://some-random-api.ml/img/whale");
                    break;
            }

            // Parse JSON into an JObject 
            JObject jObject = JObject.Parse(json);

            // Get link value
            string link = (string)jObject["link"];

            // Return the link
            return link;
        }
    }
}