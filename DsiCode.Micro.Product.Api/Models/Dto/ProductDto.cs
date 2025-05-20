namespace DsiCode.Micro.Product.Api.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public String ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }

        public IFormFile Image { get; set; }


        //IFormFile aydua a traer todos los datos del archivo, nombre, esxtension, peso, etc

    }
}
