using IVP_CS_SecurityMaster.Security_Repository.EquityRepository;
using Microsoft.AspNetCore.Mvc;
using Model_Library;

namespace IVP_CS_SecurityMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquityController : ControllerBase
    {
        private readonly IEquity _equity;

        public EquityController(IEquity equity)
        {
            _equity = equity;
        }

        [HttpPut("UpdateEquity/{id}")]
        public async Task<IActionResult> UpdateEquity([FromBody] UpdateEquitySecurity eqm, [FromRoute] int id) 
        {
            var res = await _equity.UpdateSecurityAsync(eqm,id);
            return Ok(res);
        }

        [HttpPost("DeleteEquitySecurity/{id}")]
        public async Task<IActionResult> DeleteEquity([FromRoute] int id)
        {
            var res = await _equity.DeleteSecurityAsync(id);
            return Ok(res);
        }

        [HttpGet("GetAllEquitySecurity")]
        public async Task<IActionResult> GetAllEquity()
        {
            var res = await _equity.GetSecurityDataAsync();
            return Ok(res);
        }
    }
}
