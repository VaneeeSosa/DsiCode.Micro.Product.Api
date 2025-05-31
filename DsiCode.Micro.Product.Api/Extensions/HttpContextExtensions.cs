using Microsoft.EntityFrameworkCore;
namespace DsiCode.Micro.Product.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParamPageHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            //contamos el total de la consulta que reciba el IQereable 
            //una vez calculando el total de registros consultados se asigna a la variable total
            double total = await queryable.CountAsync();

            httpContext.Response.Headers.Append("Cantidad-total-registros", total.ToString());
        }
    }
}
