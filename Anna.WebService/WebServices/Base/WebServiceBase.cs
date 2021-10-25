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

using Anna.WebService.WebServices.Base.Interface;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Anna.WebService.WebServices.Base
{
    public abstract class WebServiceBase : IWebServiceBase
    {
        #region CallWebApiAsync
        /// <summary>
        /// Calls a given endpoint and returns a string with the retrieved json data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual async Task<string> CallWebApiAsync(string url)
        {
            try
            {
                // Create a HttpWebRequest
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(url);
                // Set method to get
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                // Set accept headers
                httpWebRequest.Accept = "application/json";

                // String for storing the json data retrieved from the endpoint
                string result;

                // Get response from the website
                using(HttpWebResponse response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                {
                    // Read the response
                    using StreamReader sr = new StreamReader(response.GetResponseStream());
                    // Assign the response data to the result variable
                    result = await sr.ReadToEndAsync();
                };

                // Return the retrieved JSON data.
                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion

        #region CallWebApi
        /// <summary>
        /// Calls a given endpoint and returns a string with the retrieved json data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string CallWebApi(string url)
        {
            try
            {
                // string for storing the result
                string result = string.Empty;

                // Use WebClient to download the data
                using(WebClient client = new WebClient())
                {
                    result = client.DownloadString(url);
                }

                // Return the retrieved data.
                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion
    }
}