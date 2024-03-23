using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Movies_Core_Layer.Interfaces;
using System.Linq.Expressions;

namespace Movies_Data_Access_Layer.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (string include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> entity = _context.Set<T>().Where(expression);
            return await entity.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> expression, string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (string include in includes)
            {
                query = query.Include(include);
            }
            query = query.Where(expression);
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(int id, string[] includes)
        {
            T entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            foreach (var include in includes)
            {
                var navigation = _context.Entry(entity).Navigation(include);
                if (navigation != null)
                {
                    if (navigation is CollectionEntry collection)
                    {
                        await collection.LoadAsync();
                    }
                    else if (navigation is ReferenceEntry reference)
                    {
                        await reference.LoadAsync();
                    }
                }
            }
            return entity;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}