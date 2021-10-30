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
using RestSharp;
using System.Threading.Tasks;
using System.Threading;
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
        public virtual async Task<(bool, string)> CallWebApiAsync(
            string url, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Instantiate new client with target endpoint
                RestClient client = new(url);

                // Instantiate new request, and set parameters
                RestRequest request = new("", Method.GET, DataFormat.Json);

                // Get response from target endpoint
                IRestResponse response = await client.ExecuteAsync(request, cancellationToken);

                // Return response content
                return (response.IsSuccessful, response.Content);
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
        public virtual (bool, string) CallWebApi(string url)
        {
            try
            {
                // Instantiate new client with target endpoint
                RestClient client = new(url);

                // Instantiate new request, and set parameters
                RestRequest request = new("", Method.GET, DataFormat.Json);

                // Get response from target endpoint
                IRestResponse response = client.Execute(request);

                // Return response content
                return (response.IsSuccessful, response.Content);
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion
    }
}