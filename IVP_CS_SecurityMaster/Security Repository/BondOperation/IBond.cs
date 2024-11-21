using Model_Library;

namespace IVP_CS_SecurityMaster.Security_Repository.BondOperation
{
    public interface IBond
    {
        public void ImportDataFromCsv(string filePath);
        public Task<string> UpdateSecurityAsync(UpdateBondSecurity model,int id);
        public Task<string> DeleteSecurityAsync(int securityId);
        //public Task<List<UpdateBondSecurity>> GetSecurityDataAsync();
        public Task<List<BondSecurity>> GetSecurityDataAsync();

    }
}
