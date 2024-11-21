using IVP_CS_SecurityMaster.AuditRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model_Library;

namespace IVP_CS_SecurityMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAudit _audit;

        public AuditController(IAudit audit)
        {
            _audit = audit;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditData()
        {
            try
            {
                var auditRecords = await _audit.GetAuditData();
                return Ok(auditRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
