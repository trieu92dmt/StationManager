using Core.Models;
using System.Linq.Expressions;

namespace Core.SeedWork
{
    public static class CustomSearch
    {
        public static List<T> CustomSearchFunc<T>(DatatableViewModel model, out int filterResultsCount, out int totalResultsCount, out int totalPagesCount, IQueryable<T> query, string sortByDefault = "", string strSortDir = null)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.limit;
            var skip = model.offset;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                var columnIndex = model.order[0].column;
                //default sort on the 1st column
                sortBy = model.columns[columnIndex].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
                if (sortByDefault != "STT" && sortBy == "STT")
                {
                    sortBy = "";
                    sortDir = (strSortDir == "asc" || strSortDir == null);
                }
            }

            var result = GetData<T>(query, searchBy, take, skip, sortBy, sortByDefault, sortDir, out filterResultsCount, out totalResultsCount, out totalPagesCount);
            if (result == null)
            {
                return new List<T>();
            }
            return result;
        }

        public static List<T> GetData<T>(IQueryable<T> list, string searchBy, int take, int skip, string sortBy, string sortByDefault, bool sortDir, out int filterResultsCount, out int totalResultsCount, out int totalPagesCount)
        {
            if (String.IsNullOrEmpty(sortBy))
            {
                sortBy = sortByDefault;
                //sortDir = true;
            }

            var param = Expression.Parameter(typeof(T), "c");
            var body = sortBy.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            IOrderedQueryable<T> result;
            if (sortBy != "STT")
            {
                result = sortDir == true ?
                (IOrderedQueryable<T>)Queryable.OrderBy(list.AsQueryable(), (dynamic)Expression.Lambda(body, param)) :
                (IOrderedQueryable<T>)Queryable.OrderByDescending(list.AsQueryable(), (dynamic)Expression.Lambda(body, param));
            }
            else
            {
                result = (IOrderedQueryable<T>)list;
            }
            List<T> res = result.Skip(skip).Take(take).ToList();

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filterResultsCount = res.Count();
            totalResultsCount = result.Count();
            totalPagesCount = totalResultsCount / take;
            if (totalResultsCount % take > 0)
            {
                totalPagesCount++;
            }
            return res;
        }
    }
}
