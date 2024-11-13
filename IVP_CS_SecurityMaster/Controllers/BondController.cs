using IVP_CS_SecurityMaster.Security_Repository.BondOperation;
using IVP_CS_SecurityMaster.Security_Repository.EquityRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model_Library;

namespace IVP_CS_SecurityMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondController : ControllerBase
    {
        private readonly IBond _bond;

        public BondController(IBond bond)
        {
            _bond = bond;
        }

        [HttpPost]

        public void AddBond()
        {
             _bond.ImportDataFromCsv(@"C:\Users\asparanjape\Downloads\Data for securities.xlsx - Bonds.csv");
            
        }

        [HttpPut("UpdateEquity")]
        public async Task<IActionResult> UpdateEquity([FromBody] UpdateBondSecurity bondm)
        {
            var res = await _bond.UpdateSecurityAsync(bondm);
            return Ok(res);
        }

        [HttpPost("DeleteEquitySecurity/{id}")]
        public async Task<IActionResult> DeleteEquity([FromRoute] int id)
        {
            var res = await _bond.DeleteSecurityAsync(id);
            return Ok(res);
        }

        [HttpGet("GetAllEquitySecurity")]
        public async Task<IActionResult> GetAllEquity()
        {
            var res = await _bond.GetSecurityDataAsync();
            return Ok(res);
        }
    }
}
