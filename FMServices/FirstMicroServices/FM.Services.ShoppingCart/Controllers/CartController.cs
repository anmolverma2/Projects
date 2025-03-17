using AutoMapper;
using FM.Services.CoupenAPI.Data;
using FM.Services.ShoppingCart.Models;
using FM.Services.ShoppingCart.Models.DTO;
using FM.Services.ShoppingCart.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace FM.Services.ShoppingCart.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : Controller
    {
        private IMapper _mapper;
        private ResponseModel _response;
        private readonly AppDBContext _dbContext;
        private IProductServices _productServices;
        private ICouponService _couponServices;
        public CartController(IMapper mapper,AppDBContext dBContext,IProductServices productServices,ICouponService couponService)
        {
            _dbContext = dBContext;
            _mapper = mapper;
            this._response = new();
            _productServices = productServices;
            this._couponServices = couponService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseModel> GetCart(string userId)
        {
            try
            {
                CartDto cart = new CartDto();

                var cartHeaderEntity = _dbContext.CartHeaders
                    .FirstOrDefault(x => x.UserId == userId);

                if (cartHeaderEntity == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Cart not found for the given user.";
                    return _response;
                }

                cart.cartHeader = _mapper.Map<CartHeaderDto>(cartHeaderEntity);

                cart.cartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(
                    _dbContext.CartDetails
                    .Where(x => x.CartHeaderId == cart.cartHeader.CartHeaderId)
                    .ToList());

                if (cart.cartDetails == null || !cart.cartDetails.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "No cart details found.";
                    return _response;
                }

                IEnumerable<ProductDto> productDtos = await _productServices.GetProductsAsync();

                foreach (var item in cart.cartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(x => x.ProductId == item.ProductId);
                    if (item.Product != null) 
                    {
                        cart.cartHeader.CartTotal += (item.Count * item.Product.Price);
                    }
                }

                if (!string.IsNullOrEmpty(cart.cartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponServices.GetCouponCode(cart.cartHeader.CouponCode);
                    if (coupon != null && cart.cartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.cartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.cartHeader.Discount = coupon.DiscountAmount;
                    } 
                }

                _response.Data = cart;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cart)
        {
            try
            {
                var cartfromdb = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == cart.cartHeader.UserId);
                cartfromdb.CouponCode = cart.cartHeader.CouponCode;
                _dbContext.CartHeaders.Update(cartfromdb);
                await _dbContext.SaveChangesAsync();
                _response.Data = true;
            }
            catch(Exception ex)
            {
                _response.IsSuccess= false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }


        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cart)
        {
            try
            {
                var cartfromdb = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == cart.cartHeader.UserId);
                cartfromdb.CouponCode = "";
                _dbContext.CartHeaders.Update(cartfromdb);
                await _dbContext.SaveChangesAsync();
                _response.Data = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }


        //[HttpPost("Upsert")]
        //public async Task<ResponseModel> Upsert(CartDto cart)
        //{
        //    try
        //    {
        //        var cartHeaderDetails = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == cart.cartHeader.UserId);
        //        if (cartHeaderDetails == null)
        //        {
        //            CartHeader cartHeader = _mapper.Map<CartHeader>(cart.cartHeader);
        //            cartHeader.CartHeaderId = 0;
        //            _dbContext.CartHeaders.Add(cartHeader);
        //            await _dbContext.SaveChangesAsync();
        //            cart.cartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
        //            _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.cartDetails.First()));
        //            await _dbContext.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            var cartDetailsDto = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
        //                x => x.ProductId == cart.cartDetails.First().ProductId 
        //                && x.CartHeaderId == cartHeaderDetails.CartHeaderId );

        //            if (cartDetailsDto == null)
        //            {
        //                cart.cartDetails.First().CartHeaderId = cartHeaderDetails.CartHeaderId;
        //                _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.cartDetails.First()));
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                cart.cartDetails.First().Count += cartDetailsDto.Count;
        //                cart.cartDetails.First().CartHeaderId = cartDetailsDto.CartHeaderId;
        //                cart.cartDetails.First().CartDetilsId = cartDetailsDto.CartDetilsId;
        //                _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cart.cartDetails.First()));
        //                await _dbContext.SaveChangesAsync();
        //            }                        
        //        }
        //        _response.Data = cart;
        //        _response.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.Message = ex.Message.ToString();
        //        _response.IsSuccess = false;
        //    }
        //    return _response;
        //}


        [HttpPost("Upsert")]
        public async Task<ResponseModel> Upsert(CartDto cart)
        {
            try
            {
                var cartHeaderDetails = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(x => x.UserId == cart.cartHeader.UserId);

                if (cartHeaderDetails == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.cartHeader);
                    _dbContext.CartHeaders.Add(cartHeader);
                    await _dbContext.SaveChangesAsync(); 

                    if (cart.cartDetails.Any())
                    {
                        cart.cartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.cartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    var cartDetailsEntity = await _dbContext.CartDetails.FirstOrDefaultAsync(
                        x => x.ProductId == cart.cartDetails.First().ProductId
                        && x.CartHeaderId == cartHeaderDetails.CartHeaderId);

                    if (cartDetailsEntity != null)
                    {
                        cartDetailsEntity.Count += cart.cartDetails.First().Count;
                        _dbContext.CartDetails.Update(cartDetailsEntity);
                    }
                    else if (cart.cartDetails.Any())
                    {
                        cart.cartDetails.First().CartHeaderId = cartHeaderDetails.CartHeaderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.cartDetails.First()));
                    }

                    await _dbContext.SaveChangesAsync();
                }

                _response.Data = cart;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseModel> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails details = _dbContext.CartDetails.First(x => x.CartDetilsId == cartDetailsId);
                int totalCountOfCartItems = _dbContext.CartDetails.Where(x => x.CartHeaderId == details.CartHeaderId).Count();

                if(totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dbContext.CartHeaders.
                        FirstOrDefaultAsync(x => x.CartHeaderId == details.CartHeaderId);
                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _dbContext.SaveChangesAsync();
                _response.Data = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
