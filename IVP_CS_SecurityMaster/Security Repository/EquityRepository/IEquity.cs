using Model_Library;

namespace IVP_CS_SecurityMaster.Security_Repository.EquityRepository
{
    public interface IEquity
    {
        public void ImportDataFromCsv(string filePath);

        public Task<string> UpdateSecurityAsync(UpdateEquitySecurity model,int id);

        public Task<string> DeleteSecurityAsync(int securityId);

        public Task<List<UpdateEquitySecurity>> GetSecurityDataAsync();

    }
}
