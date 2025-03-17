using AutoMapper;
using FM.Services.CoupenAPI.Data;
using FM.Services.CoupenAPI.Models;
using FM.Services.CoupenAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FM.Services.CoupenAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDBContext _dBContext;
        private readonly ResponseModel _responseModel;
        private readonly IMapper _mapper;
        public CouponController(AppDBContext dBContext,IMapper mapper)
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
                var data =_dBContext.Coupens.ToList();
               _responseModel.Data = _mapper.Map<List<CouponDTO>>(data);
                
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
        public object Post([FromBody]CouponDTO coupon)
        {
            if(coupon != null)
            {
                var data = _mapper.Map<CouponModel>(coupon);
                _dBContext.Coupens.Add(data);
                _dBContext.SaveChanges();
                coupon.CouponId=data.CouponId;

                _responseModel.Data = coupon;
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
                var res = _dBContext.Coupens.Where(n => n.CouponId == id).FirstOrDefault();
                
                _responseModel.Data = _mapper.Map<CouponDTO>(res);
                _responseModel.Message = "Success";

                return _responseModel;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        [HttpGet("GetbyCode/{code}")]
        public object GetByCode(string code)
        {
            try
            {
                var res = _dBContext.Coupens.Where(n => n.CoupenCode.ToLower().Contains(code.ToLower())).FirstOrDefault();
                _responseModel.Data = _mapper.Map<CouponDTO>(res);
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
        public object Put([FromBody] CouponDTO coupon)
        {
            var res = _dBContext.Coupens.Where(n => n.CouponId == coupon.CouponId).AsNoTracking().FirstOrDefault();
            if(res != null)
            {
                var data = _mapper.Map<CouponModel>(coupon);
                _dBContext.Coupens.Update(data);
                _dBContext.SaveChanges();
                _responseModel.Data = coupon;
                _responseModel.Message = "Success";
            }
            return _responseModel;
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public object Delete(int id)
        {
            var data = _dBContext.Coupens.Where(n => n.CouponId == id).FirstOrDefault();
            if (data != null)
            {
                _dBContext.Coupens.Remove(data);
                _dBContext.SaveChanges();
                _responseModel.Data = data;
                _responseModel.IsSuccess = true;
                _responseModel.Message = "Success";
            }
            return _responseModel;
        }
        
    }
}
