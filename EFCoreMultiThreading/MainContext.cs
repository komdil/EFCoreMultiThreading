using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreMultiThreading
{
    public class MainContext : DbContext
    {
        public MainContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = localhost;Initial Catalog=MultiTHreading;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var student = modelBuilder.Entity<Student>();
            student.HasKey(s => s.Guid);
            student.HasOne(s => s.School).WithMany(s => s.Students).HasForeignKey(s => s.SchoolGuid);
            student.HasMany(s => s.Backpacks).WithOne(s => s.Student).HasForeignKey(s => s.StudentGuid);

            var school = modelBuilder.Entity<School>();
            school.HasKey(s => s.Guid);
            school.HasMany(s => s.Students).WithOne(s => s.School).HasForeignKey(s => s.SchoolGuid);

            var backacb = modelBuilder.Entity<Backpack>();
            backacb.HasKey(s => s.Guid);
            backacb.HasOne(s => s.Student).WithMany(s => s.Backpacks).HasForeignKey(s => s.StudentGuid);
            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<T> GetEntities<T>() where T : class
        {
            var sourceQuery = Set<T>();
            var query = new EagleFrameworkDbSet<T>(this, sourceQuery);
            return query;
        }
    }

    public class EagleFrameworkDbSet<T> : DbSet<T>, IQueryable<T> where T : class
    {
        public DbContext Context { get; }
        public DbSet<T> Source { get; }

        public Type ElementType => Source.AsQueryable().ElementType;

        public Expression Expression => Source.AsQueryable().Expression;

        public IQueryProvider Provider { get; }

        public EagleFrameworkDbSet(DbContext dbContext, DbSet<T> dbSet)
        {
            Context = dbContext;
            Source = dbSet;
            Provider = new QueryProvider(dbContext, dbSet.AsQueryable().Provider);
        }
        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                Monitor.Enter(Context);
                foreach (var item in Source)
                {
                    yield return item;
                }
            }
            finally
            {
                Monitor.Exit(Context);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region DbSet ovveride

        public override EntityEntry<T> Add(T entity)
        {
            return Source.Add(entity);
        }

        public override ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            return Source.AddAsync(entity, cancellationToken);
        }

        public override void AddRange(IEnumerable<T> entities)
        {
            Source.AddRange(entities);
        }

        public override void AddRange(params T[] entities)
        {
            Source.AddRange(entities);
        }

        public override Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return Source.AddRangeAsync(entities, cancellationToken);
        }

        public override Task AddRangeAsync(params T[] entities)
        {
            return Source.AddRangeAsync(entities);
        }

        public override IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return Source.AsAsyncEnumerable();
        }

        public override IQueryable<T> AsQueryable()
        {
            return Source.AsQueryable();
        }

        public override EntityEntry<T> Attach(T entity)
        {
            return Source.Attach(entity);
        }

        public override void AttachRange(IEnumerable<T> entities)
        {
            Source.AttachRange(entities);
        }

        public override void AttachRange(params T[] entities)
        {
            Source.AttachRange(entities);
        }

        public override IEntityType EntityType => Source.EntityType;

        public override bool Equals(object obj)
        {
            return Source.Equals(obj);
        }

        public override T Find(params object[] keyValues)
        {
            return Source.Find(keyValues);
        }

        public override ValueTask<T> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return Source.FindAsync(keyValues, cancellationToken);
        }

        public override void UpdateRange(params T[] entities)
        {
            Source.UpdateRange(entities);
        }

        public override ValueTask<T> FindAsync(params object[] keyValues)
        {
            return Source.FindAsync(keyValues);
        }

        public override void UpdateRange(IEnumerable<T> entities)
        {
            Source.UpdateRange(entities);
        }

        public override EntityEntry<T> Remove(T entity)
        {
            return Source.Remove(entity);
        }

        public override LocalView<T> Local => Source.Local;

        public override void RemoveRange(IEnumerable<T> entities)
        {
            Source.RemoveRange(entities);
        }

        public override void RemoveRange(params T[] entities)
        {
            Source.RemoveRange(entities);
        }

        public override EntityEntry<T> Update(T entity)
        {
            return Source.Update(entity);
        }
        public override int GetHashCode()
        {
            return Source.GetHashCode();
        }

        public override string ToString()
        {
            return Source.ToString();
        }

        #endregion
    }

    public class QueryProvider : IQueryProvider, IAsyncQueryProvider
    {

        public DbContext Context { get; }
        public IQueryProvider Provider { get; }

        public QueryProvider(DbContext dbContext, IQueryProvider source)
        {
            Context = dbContext;
            Provider = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var query = Provider.CreateQuery(expression);

            return query;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var query = new EntityQueryable<TElement>(this, expression);
            return query;
        }

        public object Execute(Expression expression)
        {
            try
            {
                Monitor.Enter(Context);
                return Provider.Execute(expression);
            }
            finally
            {
                Monitor.Exit(Context);
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            try
            {
                Monitor.Enter(Context);
                return Provider.Execute<TResult>(expression);
            }
            finally
            {
                Monitor.Exit(Context);
            }
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                Monitor.Enter(Context);
                return Provider.Execute<TResult>(expression);
            }
            finally
            {
                Monitor.Exit(Context);
            }
        }
    }
}
