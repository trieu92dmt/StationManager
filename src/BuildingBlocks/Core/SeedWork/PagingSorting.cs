using System.Reflection;

namespace Core.SeedWork
{
    public static class PagingSorting
    {
        public static IQueryable<T> Sorting<T>(PagingQuery searchRequest, IQueryable<T> query)
        {
            if (string.IsNullOrEmpty(searchRequest.OrderByDesc) && string.IsNullOrEmpty(searchRequest.OrderBy))
            {
                return query;
            }
            else
            {
                //Check field muốn orderby có tồn tại trong response không.
                Type fieldsType = typeof(T);
                PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                //Danh sách field
                var listFied = props.Select(x => x.Name).ToList();

                //Check request order by desc
                var checkExistOrderByDesc = string.IsNullOrEmpty(searchRequest.OrderByDesc) ? new List<string>() :
                                            listFied.Where(x => x.ToLower().Trim().Contains(searchRequest.OrderByDesc.ToLower().Trim())).ToList();

                //Check request order by
                var checkExistOrderBy = string.IsNullOrEmpty(searchRequest.OrderBy) ? new List<string>() :
                                        listFied.Where(x => x.ToLower().Trim().Contains(searchRequest.OrderBy.ToLower().Trim())).ToList();

                if (!checkExistOrderBy.Any() && !checkExistOrderBy.Any())
                    return query;
            }

            if (!string.IsNullOrEmpty(searchRequest.OrderBy))
            {
                string sortField = searchRequest.OrderBy.ToLower();
                query = query.OrderBy(sortField);
            }
            else if (!string.IsNullOrEmpty(searchRequest.OrderByDesc))
            {
                string sortField = searchRequest.OrderByDesc.ToLower();
                query = query.OrderByDescending(sortField);
            }

            return query;
        }
    }
}
