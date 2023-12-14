using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.API.Controllers.Services;
using StockMarket.DataAccess.Context;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionRepository _commissionRepository;
        public CommissionController(ICommissionRepository commissionRepository)
        {
            _commissionRepository = commissionRepository;
        }

        [HttpGet("GetCommissions/")]
        public async Task<ActionResult<List<Commission>>> GetCommissions()
        {
            return await _commissionRepository.GetAll();
        }
    }
}
