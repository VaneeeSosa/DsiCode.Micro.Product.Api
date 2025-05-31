namespace DsiCode.Micro.Product.Api.Models.Dto
{
    public record PagerDto(int Page=1, int RecordsPerPage=10)
        //aqui se indica la paginacion, inicia en 1
    {
        private const int MaxRecordPerPage = 50;
        public int Page { get; set; } = Math.Max(1,Page);
        //aqui se hace la paginacion, su valor maximo siempre sera 1, si el usuario ingresa un -1, el 1 siempre sera mayor
        public int RecordsPerPage { get; init; }=Math.Clamp(RecordsPerPage,1,MaxRecordPerPage);
        //clamp me permite identificar un vaor valido entre 1 y el valor maximo 
    }
}
