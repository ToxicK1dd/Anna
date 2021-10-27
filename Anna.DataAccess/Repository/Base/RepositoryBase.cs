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

using Anna.DataAccess.Repository.Base.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Anna.DataAccess.Repository.Base
{
    public abstract class RepositoryBase<TModel, TContext> : IRepositoryBase<TModel> where TModel : class
        where TContext : DbContext, new()
    {
        protected TContext context;

        public RepositoryBase(TContext context)
        {
            this.context = context;
        }


        public virtual async Task AddAsync(
            TModel model,
            CancellationToken cancellationToken = default)
        {
            await context.Set<TModel>().AddAsync(model, cancellationToken);
        }


        public virtual async Task<TModel> FirstOrDefaultAsync(
            Expression<Func<TModel, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<TModel> FirstOrDefaultAsync(
            Expression<Func<TModel, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TModel, object>>[] includes)
        {
            IQueryable<TModel> query = context.Set<TModel>().AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<TModel> FirstOrDefaultAsync(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TModel>> selector,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>()
                .Where(predicate)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }


        public virtual IEnumerable<TModel> Get(
            Expression<Func<TModel, bool>> predicate)
        {
            return context.Set<TModel>().Where(predicate);
        }

        public virtual async Task<TModel> GetByIdAsync(
            ulong id,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>().FindAsync(id, cancellationToken);
        }

        public virtual IEnumerable<TModel> GetAll()
        {
            return context.Set<TModel>().AsQueryable();
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync(
            Expression<Func<TModel, TModel>> keySelector,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>()
                .OrderBy(keySelector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync(
            Expression<Func<TModel, TModel>> keySelector,
            Expression<Func<TModel, bool>> predicate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>()
                .Where(predicate)
                .OrderBy(keySelector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }


        public virtual void Remove(TModel t)
        {
            context.Set<TModel>().Remove(t);
        }

        public virtual async Task<bool> ExistsAsync(
            ulong id,
            CancellationToken cancellationToken = default)
        {
            return await context.Set<TModel>().FindAsync(id, cancellationToken) != null;
        }
    }
}