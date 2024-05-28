using EcommerceAPI.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.UnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        protected DbContext Context { get; }

        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await Context.SaveChangesAsync(cancellationToken);

        //public virtual async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default)
        //    => await Context.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }
}