using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceAPI.Core.Extentions;
using System.Linq.Expressions;

namespace EcommerceAPI.Core.Extentions
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }
        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
        public static IQueryable<T2> To<T1, T2>(this IQueryable<T1> source, IMapper mapper)
        {
            return source.ProjectTo<T2>(mapper.ConfigurationProvider);
        }
        public static IQueryable<T> To<T>(this IQueryable source, IMapper mapper)
        {
            return source.ProjectTo<T>(mapper.ConfigurationProvider);
        }
    }
}