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

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace Anna.DataAccess.Repository.Base.Interface
{
    public interface IRepositoryBase<TModel>
    {
        /// <summary>
        /// Add a <see cref="TModel"/> to the database.
        /// </summary>
        /// <param name="t"></param>
        Task AddAsync(TModel t);

        /// <summary>
        /// Get an object by predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TModel> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Get an object by predicate, and with specified navigation properties.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        Task<TModel> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate,
            params Expression<Func<TModel, object>>[] includeProperties);

        /// <summary>
        /// Get all objects by predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TModel> Get(Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Get all objects.
        /// </summary>
        /// <returns>All objects</returns>
        IEnumerable<TModel> GetAll();

        /// <summary>
        /// Get an object by id.
        /// </summary>
        /// <param name="id"></param>
        Task<TModel> GetByIdAsync(ulong id);

        /// <summary>
        /// Removes an object.
        /// and saves the changes.
        /// </summary>
        /// <param name="t"></param>
        void Remove(TModel t);

        /// <summary>
        /// Check if an object with the specified id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A boolean value indicating the existance of an object</returns>
        Task<bool> ExistsAsync(ulong id);
    }
}