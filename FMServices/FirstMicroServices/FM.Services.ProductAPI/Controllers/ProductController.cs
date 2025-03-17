using AutoMapper;
using FM.Services.ProductAPI.Data;
using FM.Services.ProductAPI.Models;
using FM.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FM.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly AppDBContext _dBContext;
        private readonly ResponseModel _responseModel;
        private readonly IMapper _mapper;
        public ProductController(AppDBContext dBContext,IMapper mapper)
        {
            _dBContext = dBContext;
            _responseModel = new();
            _mapper = mapper;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                var data =_dBContext.Products.ToList();
               _responseModel.Data = _mapper.Map<List<ProductDto>>(data);
                
                _responseModel.Message = "Data retrieved";
                
                return _responseModel;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public object Post([FromBody]ProductDto product)
        {
            if(product != null)
            {
                var data = _mapper.Map<Product>(product);
                _dBContext.Products.Add(data);
                _dBContext.SaveChanges();
                product.ProductId=data.ProductId;

                _responseModel.Data = product;
                _responseModel.Message = "Success";
                _responseModel.IsSuccess = true;
            }
            return _responseModel;
        }

        [HttpGet("{id:int}")]
        public object Get(int id)
        {
            try
            {
                var res = _dBContext.Products.Where(n => n.ProductId == id).FirstOrDefault();
                
                _responseModel.Data = _mapper.Map<ProductDto>(res);
                _responseModel.Message = "Success";

                return _responseModel;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public object Put([FromBody] ProductDto product)
        {
            var res = _dBContext.Products.Where(n => n.ProductId == product.ProductId).AsNoTracking().FirstOrDefault();
            if(res != null)
            {
                var data = _mapper.Map<Product>(product);
                _dBContext.Products.Update(data);
                _dBContext.SaveChanges();
                _responseModel.Data = product;
                _responseModel.Message = "Success";
            }
            return _responseModel;
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public object Delete(int id)
        {
            var data = _dBContext.Products.Where(n => n.ProductId == id).FirstOrDefault();
            if (data != null)
            {
                _dBContext.Products.Remove(data);
                _dBContext.SaveChanges();
                _responseModel.Data = data;
                _responseModel.IsSuccess = true;
                _responseModel.Message = "Success";
            }
            return _responseModel;
        }
        
    }
}
