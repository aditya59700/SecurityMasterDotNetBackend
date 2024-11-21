using Model_Library;

namespace IVP_CS_SecurityMaster.AuditRepository
{
    public interface IAudit
    {
        public Task<List<AuditData>> GetAuditData();

    }
}
