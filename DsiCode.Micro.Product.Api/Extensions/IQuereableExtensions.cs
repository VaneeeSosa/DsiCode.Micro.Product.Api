using DsiCode.Micro.Product.Api.Models.Dto;

namespace DsiCode.Micro.Product.Api.Extensions
{
    public static class IQuereableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PagerDto pagerDto)
        {
            return queryable.Skip((pagerDto.Page - 1) * pagerDto.RecordsPerPage).Take(pagerDto.RecordsPerPage);
        }
    }
}
