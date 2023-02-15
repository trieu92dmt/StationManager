using Microsoft.EntityFrameworkCore;

namespace Core.SeedWork
{
    public class PaginatedList<T> : List<T>
    {
        public int Limit { get; private set; }
        public int Offset { get; private set; }
        public int Total { get; private set; }

        public PaginatedList(List<T> items, int count, int offset, int limit)
        {
            Limit = limit;
            Offset = offset;
            Total = count;

            this.AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int offset = 0, int limit = 20)
        {
            var count = await source.CountAsync();
            if (limit == -1)
            {
                limit = count;
                return new PaginatedList<T>(source.ToList(), count, 0, limit);
            }
            else
            {
                var items = await source
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
                return new PaginatedList<T>(items, count, offset, limit);
            }
        }


        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> action, int offset, int limit)
        {
            var count = await source.CountAsync();
            if (limit == -1)
            {
                limit = count;
                return new PaginatedList<T>(await action(source).ToListAsync(), count, 0, limit);
            }
            else
            {
                IQueryable<T> items = source
                .Skip(offset)
                .Take(limit);
                return new PaginatedList<T>(await action(items).ToListAsync(), count, offset, limit);
            }
        }

        public static PaginatedList<T> Create(
           IQueryable<T> source, int offset = 0, int limit = 20)
        {
            var count = source.Count();
            if (limit == -1)
            {
                limit = count;
                return new PaginatedList<T>(source.ToList(), count, 0, limit);
            }
            else
            {
                var items = source
                .Skip(offset)
                .Take(limit)
                .ToList();
                return new PaginatedList<T>(items, count, offset, limit);
            }
        }
    }
}
