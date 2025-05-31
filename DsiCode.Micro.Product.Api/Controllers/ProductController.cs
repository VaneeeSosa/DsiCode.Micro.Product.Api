using AutoMapper;
using DsiCode.Micro.Product.Api.Data;
using DsiCode.Micro.Product.Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DsiCode.Micro.Product.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        // GET: ProductController
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public ProductController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<DsiCode.Micro.Product.Api.Models.Product> objList =
                     _db.Productos.ToList();
                _response.Result = _mapper.Map<List<DsiCode.Micro.Product.Api.Models.Dto.ProductDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        //seguridad de autorizacion
        //tipo mime (audio, documento, img, etc)
        //viene como objeto 
        [Authorize(Roles = "ADMINISTRADOR")]
        public ResponseDto Post(ProductDto productDto)
        {
            try
            {
                Product.Api.Models.Product product =
                _mapper.Map<Product.Api.Models.Product>(productDto);
                _db.Productos.Add(product);
                _db.SaveChanges();

                if (productDto.Image != null)
                {
                    // Logic to handle image upload
                    string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                    var filePath = @"wwwroot\images\" + fileName;
                    var fulePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(fulePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _db.Productos.Update(product);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(productDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                DsiCode.Micro.Product.Api.Models.Product product =
                    _db.Productos.FirstOrDefault(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMINISTRADOR")]
        public ResponseDto Put(ProductDto ProductDto)
        {
            try
            {
                Product.Api.Models.Product product =
                    _mapper.Map<Product.Api.Models.Product>(ProductDto);
                if (ProductDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldfilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                        FileInfo archivo = new FileInfo(oldfilePathDirectory);
                        if (archivo.Exists)
                        {
                            archivo.Delete();
                        }
                    }
                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string archivoPath = @"wwwroot\images\" + fileName;
                    var archivoPathDirectory = Path.Combine(Directory.GetCurrentDirectory(), archivoPath);
                    using (var fileStream = new FileStream(archivoPathDirectory, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/images" + fileName;

                    product.ImageLocalPath = archivoPath;
                }
                _db.Productos.Update(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product.Api.Models.Product product = _db.Productos.First(p => p.ProductId == id);
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldPathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo archivo = new FileInfo(oldPathDirectory);
                    if (archivo.Exists)
                    {
                        archivo.Delete();
                    }
                }
                _db.Productos.Remove(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;

        }
    }
}