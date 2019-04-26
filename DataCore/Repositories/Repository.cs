﻿//===================================================================================
// Microsoft Developer & Platform Evangelism
//=================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
//===================================================================================


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Swaksoft.Domain.Seedwork;
using Swaksoft.Domain.Seedwork.Specification;
using Swaksoft.Domain.Seedwork.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Swaksoft.Infrastructure.Data.Seedwork.Repositories
{
    /// <summary>
    /// Repository base class
    /// </summary>
    /// <typeparam name="TEntity">The type of underlying entity in this repository</typeparam>
    public class Repository<TEntity> :IRepository<TEntity>
        where TEntity:class
    {

        #region Members

        readonly IUnitOfWork _unitOfWork;

        protected DbContext Context
        {
            get
            {
                return _unitOfWork as DbContext;
            }
        }

        protected void Attach(TEntity item)            
        {
            //attach and set as unchanged
            Context.Entry(item).State = EntityState.Unchanged;
        }
        
        protected void SetModified(TEntity item)           
        {
            //this operation also attach item in object state manager
            Context.Entry(item).State = EntityState.Modified;
        }

        protected void ApplyCurrentValues(TEntity original, TEntity current)           
        {
            //if it is not attached, attach original and set current values
            Context.Entry(original).CurrentValues.SetValues(current);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public Repository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }

        #endregion

        #region IRepository Members

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }

        public virtual void Add(TEntity item)
        {
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			GetSet().Add(item); // add new item in this set            
        }

        public virtual void Remove(TEntity item)
        {
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			
            //attach item if not exist
            Attach(item);

            //set as "removed"
            GetSet().Remove(item);            
        }

        public virtual void TrackItem(TEntity item)
        {
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			
             Attach(item);
        }
      
        public virtual void Modify(TEntity item)
        {
			if (item == null) {
				throw new ArgumentNullException(nameof(item));
			}

			SetModified(item);
        }
     
        public virtual TEntity Get(int id)
        {
            return id > 0 ? GetSet().Find(id) : null;
        }

       
        public virtual IQueryable<TEntity> GetAll()
        {
            return GetQuery();
        }
        
        public virtual IQueryable<TEntity> AllMatching(ISpecification<TEntity> specification)
        {
            return GetQuery().Where(specification.SatisfiedBy());
        }

        public virtual async Task<IList<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification)
        {
            return await AllMatching(specification).ToListAsync();
        }

      
        public virtual IQueryable<TEntity> GetPaged<KProperty>(int pageIndex, int pageCount, Expression<Func<TEntity, KProperty>> orderByExpression, bool ascending)
        {
            var set = GetQuery();

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            return set.OrderByDescending(orderByExpression)
                .Skip(pageCount * pageIndex)
                .Take(pageCount);
        }
    
        public virtual IQueryable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter)
        {
            return GetQuery().Where(filter);
        }

        public async Task<IList<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await GetFiltered(filter).ToListAsync();
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            return GetQuery().SingleOrDefault(filter);
        }

        public virtual void Merge(TEntity persisted, TEntity current)
        {
            ApplyCurrentValues(persisted, current);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
			if (!disposing) return;

            if (_unitOfWork != null)
                _unitOfWork.Dispose();    
        }

        #endregion

        #region Private Methods

        protected virtual IQueryable<TEntity> GetQuery()
        {
            return Context.Set<TEntity>();
        }
        protected DbSet<TEntity> GetSet()
        {
            return Context.Set<TEntity>();
        }
        #endregion
    }
}
