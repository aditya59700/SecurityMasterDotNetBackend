using CsvHelper;
using Microsoft.Data.SqlClient;
using Model_Library;
using System.Data;
using System.Globalization;

namespace IVP_CS_SecurityMaster.Security_Repository.EquityRepository
{
    public class EquityOperation : IEquity
    {
        string connStr = @"Server = 192.168.0.13\\sqlexpress,49753; Database = IVP_SM_AA; User ID = sa; Password = sa@12345678; TrustServerCertificate = True";

        //private readonly string _connectionString;

        //public SecurityOperation(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}


        public void ImportDataFromCsv(string filePath)
        {

            var records = ReadCsvFile(filePath);

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var record in records)
                        {
                            InsertFullSecurityData(record, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General Error during import: " + ex.Message);
                        transaction.Rollback();
                    }
                }
            }
        }

        private List<EquitySecurity> ReadCsvFile(string filePath)
        {
            var records = new List<EquitySecurity>();

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    

                    // Read records with error handling
                    records = csv.GetRecords<EquitySecurity>().ToList();
                }
           
            return records;
        }

        private void ValidateSecurityData(EquitySecurity data)
        {
            Console.WriteLine(data.IsActive);
            if (data.HasPosition.HasValue && !(data.HasPosition == true || data.HasPosition == false))
            {
                Console.WriteLine($"Invalid value for HasPosition: {data.HasPosition}. Expected a boolean value.");
                throw new FormatException($"Invalid value for HasPosition: {data.HasPosition}. Expected a boolean value.");
            }

            if (data.IsActive.HasValue && !(data.IsActive == true || data.IsActive == false))
            {
                Console.WriteLine($"Invalid value for HasPosition: {data.IsActive}. Expected a boolean value.");

                throw new FormatException($"Invalid value for IsActive: {data.IsActive}. Expected a boolean value.");
            }

            if (data.IsADR.HasValue && !(data.IsADR == true || data.IsADR == false))
            {
                throw new FormatException($"Invalid value for IsADR: {data.IsADR}. Expected a boolean value.");
            }

            if (data.SharesPerADR.HasValue && !int.TryParse(data.SharesPerADR.ToString(), out _))
            {
                throw new FormatException($"Invalid value for SharesPerADR: {data.SharesPerADR}. Expected an integer.");
            }

            if (data.SettleDays.HasValue && !int.TryParse(data.SettleDays.ToString(), out _))
            {
                throw new FormatException($"Invalid value for SettleDays: {data.SettleDays}. Expected an integer.");
            }

            if (data.SharesOutstanding.HasValue && !decimal.TryParse(data.SharesOutstanding.ToString(), out _))
            {
                throw new FormatException($"Invalid value for SharesOutstanding: {data.SharesOutstanding}. Expected a decimal value.");
            }

            if (data.VotingRightsPerShare.HasValue && !decimal.TryParse(data.VotingRightsPerShare.ToString(), out _))
            {
                throw new FormatException($"Invalid value for VotingRightsPerShare: {data.VotingRightsPerShare}. Expected a decimal value.");
            }

            if (data.AverageVolume20D.HasValue && !decimal.TryParse(data.AverageVolume20D.ToString(), out _))
            {
                throw new FormatException($"Invalid value for AverageVolume20D: {data.AverageVolume20D}. Expected a decimal value.");
            }

            if (data.Beta.HasValue && !decimal.TryParse(data.Beta.ToString(), out _))
            {
                throw new FormatException($"Invalid value for Beta: {data.Beta}. Expected a decimal value.");
            }

            if (data.ShortInterest.HasValue && !decimal.TryParse(data.ShortInterest.ToString(), out _))
            {
                throw new FormatException($"Invalid value for ShortInterest: {data.ShortInterest}. Expected a decimal value.");
            }

            if (data.YTDReturn.HasValue && !decimal.TryParse(data.YTDReturn.ToString(), out _))
            {
                throw new FormatException($"Invalid value for YTDReturn: {data.YTDReturn}. Expected a decimal value.");
            }

            if (data.Volatility90D.HasValue && !decimal.TryParse(data.Volatility90D.ToString(), out _))
            {
                throw new FormatException($"Invalid value for Volatility90D: {data.Volatility90D}. Expected a decimal value.");
            }

            if (data.OpenPrice.HasValue && !decimal.TryParse(data.OpenPrice.ToString(), out _))
            {
                throw new FormatException($"Invalid value for OpenPrice: {data.OpenPrice}. Expected a decimal value.");
            }

            if (data.ClosePrice.HasValue && !decimal.TryParse(data.ClosePrice.ToString(), out _))
            {
                throw new FormatException($"Invalid value for ClosePrice: {data.ClosePrice}. Expected a decimal value.");
            }

            if (data.Volume.HasValue && !decimal.TryParse(data.Volume.ToString(), out _))
            {
                throw new FormatException($"Invalid value for Volume: {data.Volume}. Expected a decimal value.");
            }

            if (data.LastPrice.HasValue && !decimal.TryParse(data.LastPrice.ToString(), out _))
            {
                throw new FormatException($"Invalid value for LastPrice: {data.LastPrice}. Expected a decimal value.");
            }

            if (data.AskPrice.HasValue && !decimal.TryParse(data.AskPrice.ToString(), out _))
            {
                throw new FormatException($"Invalid value for AskPrice: {data.AskPrice}. Expected a decimal value.");
            }

            if (data.BidPrice.HasValue && !decimal.TryParse(data.BidPrice.ToString(), out _))
            {
                throw new FormatException($"Invalid value for BidPrice: {data.BidPrice}. Expected a decimal value.");
            }

            if (data.PERatio.HasValue && !decimal.TryParse(data.PERatio.ToString(), out _))
            {
                throw new FormatException($"Invalid value for PERatio: {data.PERatio}. Expected a decimal value.");
            }

            if (data.Amount.HasValue && !decimal.TryParse(data.Amount.ToString(), out _))
            {
                throw new FormatException($"Invalid value for Amount: {data.Amount}. Expected a decimal value.");
            }

            if (data.DeclaredDate.HasValue && data.DeclaredDate.Value > DateTime.Now)
            {
                throw new FormatException($"Invalid value for DeclaredDate: {data.DeclaredDate}. Date cannot be in the future.");
            }

            if (data.ExDate.HasValue && data.ExDate.Value > DateTime.Now)
            {
                throw new FormatException($"Invalid value for ExDate: {data.ExDate}. Date cannot be in the future.");
            }

            if (data.RecordDate.HasValue && data.RecordDate.Value > DateTime.Now)
            {
                throw new FormatException($"Invalid value for RecordDate: {data.RecordDate}. Date cannot be in the future.");
            }

            if (data.PayDate.HasValue && data.PayDate.Value > DateTime.Now)
            {
                throw new FormatException($"Invalid value for PayDate: {data.PayDate}. Date cannot be in the future.");
            }
        }

        private void InsertFullSecurityData(EquitySecurity data, SqlConnection connection, SqlTransaction transaction)
        {
            int newSecurityID = 0;
            using (SqlCommand command = new SqlCommand("Master.Insert_Equity_Security", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;

                // Security Summary Params
                command.Parameters.AddWithValue("@Security_Name", data.SecurityName);
                command.Parameters.AddWithValue("@Security_Description", data.SecurityDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Has_Position", data.HasPosition ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Is_Active", data.IsActive ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Round_Lot_Size", data.RoundLotSize ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Unique_Name", data.BloombergUniqueName ?? (object)DBNull.Value);

                // Security Identifiers
                command.Parameters.AddWithValue("@CUSIP", data.CUSIP ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ISIN", data.ISIN ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SEDOL", data.SEDOL ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Ticker", data.BloombergTicker ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Unique_ID", data.BloombergUniqueID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Global_ID", data.BloombergGlobalID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Ticker_And_Exchange", data.BloombergTickerAndExchange ?? (object)DBNull.Value);

                // Security Details
                command.Parameters.AddWithValue("@Is_ADR", data.IsADR ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ADR_Underlying_Ticker", data.ADRUnderlyingTicker ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ADR_Underlying_Currency", data.ADRUnderlyingCurrency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Shares_Per_ADR", data.SharesPerADR ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IPO_Date", data.IPODate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price_Currency", data.PriceCurrency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Settle_Days", data.SettleDays ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Shares_Outstanding", data.SharesOutstanding ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Voting_Rights_Per_Share", data.VotingRightsPerShare ?? (object)DBNull.Value);

                // Risk Details
                command.Parameters.AddWithValue("@Average_Volume_20D", data.AverageVolume20D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Beta", data.Beta ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Short_Interest", data.ShortInterest ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@YTD_Return", data.YTDReturn ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Volatility_90D", data.Volatility90D ?? (object)DBNull.Value);

                // Regulatory Details
                command.Parameters.AddWithValue("@PF_Asset_Class", data.PFAssetClass ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Country", data.PFCountry ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Credit_Rating", data.PFCreditRating ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Currency", data.PFCreditRating ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Instrument", data.PFInstrument ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Liquidity_Profile", data.PFLiquidityProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Maturity", data.PFMaturity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_NAICS_Code", data.PFNAICSCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Region", data.PFRegion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Sector", data.PFSector ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PF_Sub_Asset_Class", data.PFSubAssetClass ?? (object)DBNull.Value);

                // Reference Data
                command.Parameters.AddWithValue("@Issue_Country", data.IssueCountry ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Exchange", data.Exchange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Issuer", data.Issuer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Issue_Currency", data.IssueCurrency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Trading_Currency", data.TradingCurrency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Industry_Sub_Group", data.BloombergIndustrySubGroup ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Industry_Group", data.BloombergIndustryGroup ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Industry_Sector", data.BloombergIndustrySector ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Country_Of_Incorporation", data.CountryOfIncorporation ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Risk_Currency", data.RiskCurrency ?? (object)DBNull.Value);

                // Pricing Details
                command.Parameters.AddWithValue("@Open_Price", data.OpenPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Close_Price", data.ClosePrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Volume", data.Volume ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Last_Price", data.LastPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Ask_Price", data.AskPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bid_Price", data.BidPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PE_Ratio", data.PERatio ?? (object)DBNull.Value);

                // Dividend History
                command.Parameters.AddWithValue("@Declared_Date", data.DeclaredDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Ex_Date", data.ExDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Record_Date", data.RecordDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Pay_Date", data.PayDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Amount", data.Amount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Frequency", data.Frequency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Dividend_Type", data.Type ?? (object)DBNull.Value);

                // Output parameter to get the New Security ID
                SqlParameter newSecurityIDParam = new SqlParameter("@New_Security_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(newSecurityIDParam);

                // Execute stored procedure
                command.ExecuteNonQuery();


                // Get the new Security ID from the output parameter
                newSecurityID = (int)newSecurityIDParam.Value;
                Console.WriteLine($"New Security ID: {newSecurityID}");
            }

            Console.WriteLine($"New Security ID: {newSecurityID}");
        }

        public async Task<string> UpdateSecurityAsync(UpdateEquitySecurity model, int id)
        {
            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("Equity.UpdateSecurity", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding the parameters
                        command.Parameters.AddWithValue("@Security_ID", model.SecurityId);
                        command.Parameters.AddWithValue("@Security_Description", (object)model.SecurityDescription ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Pricing_Currency", (object)model.PricingCurrency ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Total_Shares_Outstanding", (object)model.TotalSharesOutstanding ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Open_Price", (object)model.OpenPrice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Close_Price", (object)model.ClosePrice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Dividend_Declared_Date", (object)model.DividendDeclaredDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PF_Credit_Rating", (object)model.PFCreditRating ?? DBNull.Value);

                        // Execute the stored procedure
                        var result = await command.ExecuteNonQueryAsync();

                        // If ExecuteNonQuery returns > 0, then it means rows were affected.
                        return "Updated Succesfully";
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (e.g., log it)
                    Console.WriteLine(ex.Message);
                    return "Error";
                }
            }
        }

        public async Task<string> DeleteSecurityAsync(int securityId)
        {
            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("Equity.DeleteSecurity", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameter for the security ID
                        command.Parameters.AddWithValue("@Security_ID", securityId);

                        // Execute the stored procedure
                        var result = await command.ExecuteNonQueryAsync();

                        // If ExecuteNonQuery returns > 0, the update was successful
                        return "Deleted Succesfully";

                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (if necessary)
                    Console.WriteLine(ex.Message);
                    return "Error   ";
                }
            }
        }

        public async Task<List<EquitySecurity>> GetSecurityDataAsync()
        {
            var equitySecurityList = new List<EquitySecurity>();

            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("Equity.Select_Equity_Security", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Step 1: Read the first result set (Security Data)
                            while (await reader.ReadAsync())
                            {
                                var equitySecurity = new EquitySecurity
                                {
                                    // Mapping SQL result columns to model properties
                                    SecurityId = reader.IsDBNull(reader.GetOrdinal("Security_Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Security_Id")),
                                    SecurityName = reader.IsDBNull(reader.GetOrdinal("Security_Name")) ? null : reader.GetString(reader.GetOrdinal("Security_Name")),
                                    SecurityDescription = reader.IsDBNull(reader.GetOrdinal("Security_Description")) ? null : reader.GetString(reader.GetOrdinal("Security_Description")),
                                    HasPosition = reader.IsDBNull(reader.GetOrdinal("Has_Position")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("Has_Position")),
                                    IsActive = reader.IsDBNull(reader.GetOrdinal("Is_Active")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("Is_Active")),
                                    RoundLotSize = reader.IsDBNull(reader.GetOrdinal("Round_Lot_Size")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Round_Lot_Size")),
                                    BloombergUniqueName = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Unique_Name")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Unique_Name")),

                                    // Security Identifiers
                                    CUSIP = reader.IsDBNull(reader.GetOrdinal("CUSIP")) ? null : reader.GetString(reader.GetOrdinal("CUSIP")),
                                    ISIN = reader.IsDBNull(reader.GetOrdinal("ISIN")) ? null : reader.GetString(reader.GetOrdinal("ISIN")),
                                    SEDOL = reader.IsDBNull(reader.GetOrdinal("SEDOL")) ? null : reader.GetString(reader.GetOrdinal("SEDOL")),
                                    BloombergTicker = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Ticker")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Ticker")),
                                    BloombergUniqueID = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Unique_ID")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Unique_ID")),
                                    BloombergGlobalID = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Global_ID")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Global_ID")),
                                    BloombergTickerAndExchange = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Ticker_And_Exchange")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Ticker_And_Exchange")),

                                    // Security Details
                                    IsADR = reader.IsDBNull(reader.GetOrdinal("Is_ADR")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("Is_ADR")),
                                    ADRUnderlyingTicker = reader.IsDBNull(reader.GetOrdinal("ADR_Underlying_Ticker")) ? null : reader.GetString(reader.GetOrdinal("ADR_Underlying_Ticker")),
                                    ADRUnderlyingCurrency = reader.IsDBNull(reader.GetOrdinal("ADR_Underlying_Currency")) ? null : reader.GetString(reader.GetOrdinal("ADR_Underlying_Currency")),
                                    SharesPerADR = reader.IsDBNull(reader.GetOrdinal("Shares_Per_ADR")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Shares_Per_ADR")),
                                    IPODate = reader.IsDBNull(reader.GetOrdinal("IPO_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("IPO_Date")),
                                    PriceCurrency = reader.IsDBNull(reader.GetOrdinal("Price_Currency")) ? null : reader.GetString(reader.GetOrdinal("Price_Currency")),
                                    SettleDays = reader.IsDBNull(reader.GetOrdinal("Settle_Days")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Settle_Days")),
                                    SharesOutstanding = reader.IsDBNull(reader.GetOrdinal("Shares_Outstanding")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Shares_Outstanding")),
                                    VotingRightsPerShare = reader.IsDBNull(reader.GetOrdinal("Voting_Rights_Per_Share")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Voting_Rights_Per_Share")),

                                    // Risk Details
                                    AverageVolume20D = reader.IsDBNull(reader.GetOrdinal("Average_Volume_20D")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Average_Volume_20D")),
                                    Beta = reader.IsDBNull(reader.GetOrdinal("Beta")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Beta")),
                                    ShortInterest = reader.IsDBNull(reader.GetOrdinal("Short_Interest")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Short_Interest")),
                                    YTDReturn = reader.IsDBNull(reader.GetOrdinal("YTD_Return")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("YTD_Return")),
                                    Volatility90D = reader.IsDBNull(reader.GetOrdinal("Volatility_90D")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Volatility_90D")),

                                    // Regulatory Details
                                    PFAssetClass = reader.IsDBNull(reader.GetOrdinal("PF_Asset_Class")) ? null : reader.GetString(reader.GetOrdinal("PF_Asset_Class")),
                                    PFCountry = reader.IsDBNull(reader.GetOrdinal("PF_Country")) ? null : reader.GetString(reader.GetOrdinal("PF_Country")),
                                    PFCurrency = reader.IsDBNull(reader.GetOrdinal("PF_Currency")) ? null : reader.GetString(reader.GetOrdinal("PF_Currency")),
                                    PFCreditRating = reader.IsDBNull(reader.GetOrdinal("PF_Credit_Rating")) ? null : reader.GetString(reader.GetOrdinal("PF_Credit_Rating")),
                                    PFInstrument = reader.IsDBNull(reader.GetOrdinal("PF_Instrument")) ? null : reader.GetString(reader.GetOrdinal("PF_Instrument")),
                                    PFLiquidityProfile = reader.IsDBNull(reader.GetOrdinal("PF_Liquidity_Profile")) ? null : reader.GetString(reader.GetOrdinal("PF_Liquidity_Profile")),
                                    PFMaturity = reader.IsDBNull(reader.GetOrdinal("PF_Maturity")) ? null : reader.GetString(reader.GetOrdinal("PF_Maturity")),
                                    PFNAICSCode = reader.IsDBNull(reader.GetOrdinal("PF_NAICS_Code")) ? null : reader.GetString(reader.GetOrdinal("PF_NAICS_Code")),
                                    PFRegion = reader.IsDBNull(reader.GetOrdinal("PF_Region")) ? null : reader.GetString(reader.GetOrdinal("PF_Region")),
                                    PFSector = reader.IsDBNull(reader.GetOrdinal("PF_Sector")) ? null : reader.GetString(reader.GetOrdinal("PF_Sector")),
                                    PFSubAssetClass = reader.IsDBNull(reader.GetOrdinal("PF_Sub_Asset_Class")) ? null : reader.GetString(reader.GetOrdinal("PF_Sub_Asset_Class")),

                                    // Reference Data
                                    IssueCountry = reader.IsDBNull(reader.GetOrdinal("Issue_Country")) ? null : reader.GetString(reader.GetOrdinal("Issue_Country")),
                                    Exchange = reader.IsDBNull(reader.GetOrdinal("Exchange")) ? null : reader.GetString(reader.GetOrdinal("Exchange")),
                                    Issuer = reader.IsDBNull(reader.GetOrdinal("Issuer")) ? null : reader.GetString(reader.GetOrdinal("Issuer")),
                                    IssueCurrency = reader.IsDBNull(reader.GetOrdinal("Issue_Currency")) ? null : reader.GetString(reader.GetOrdinal("Issue_Currency")),
                                    TradingCurrency = reader.IsDBNull(reader.GetOrdinal("Trading_Currency")) ? null : reader.GetString(reader.GetOrdinal("Trading_Currency")),
                                    BloombergIndustrySubGroup = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Industry_Sub_Group")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Industry_Sub_Group")),
                                    BloombergIndustryGroup = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Industry_Group")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Industry_Group")),
                                    BloombergIndustrySector = reader.IsDBNull(reader.GetOrdinal("Bloomberg_Industry_Sector")) ? null : reader.GetString(reader.GetOrdinal("Bloomberg_Industry_Sector")),
                                    CountryOfIncorporation = reader.IsDBNull(reader.GetOrdinal("Country_Of_Incorporation")) ? null : reader.GetString(reader.GetOrdinal("Country_Of_Incorporation")),
                                    RiskCurrency = reader.IsDBNull(reader.GetOrdinal("Risk_Currency")) ? null : reader.GetString(reader.GetOrdinal("Risk_Currency")),

                                    // Pricing Details
                                    OpenPrice = reader.IsDBNull(reader.GetOrdinal("Open_Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Open_Price")),
                                    ClosePrice = reader.IsDBNull(reader.GetOrdinal("Close_Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Close_Price")),
                                    Volume = reader.IsDBNull(reader.GetOrdinal("Volume")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Volume")),
                                    LastPrice = reader.IsDBNull(reader.GetOrdinal("Last_Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Last_Price")),
                                    AskPrice = reader.IsDBNull(reader.GetOrdinal("Ask_Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Ask_Price")),
                                    BidPrice = reader.IsDBNull(reader.GetOrdinal("Bid_Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Bid_Price")),
                                    PERatio = reader.IsDBNull(reader.GetOrdinal("PE_Ratio")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("PE_Ratio")),

                                    DeclaredDate = reader.IsDBNull(reader.GetOrdinal("Declared_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Declared_Date")),
                                    ExDate = reader.IsDBNull(reader.GetOrdinal("Ex_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Ex_Date")),
                                    RecordDate = reader.IsDBNull(reader.GetOrdinal("Record_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Record_Date")),
                                    PayDate = reader.IsDBNull(reader.GetOrdinal("Pay_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Pay_Date")),
                                    Amount = reader.IsDBNull(reader.GetOrdinal("Amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Amount")),
                                    Frequency = reader.IsDBNull(reader.GetOrdinal("Frequency")) ? null : reader.GetString(reader.GetOrdinal("Frequency")),
                                    Type = reader.IsDBNull(reader.GetOrdinal("Dividend_Type")) ? null : reader.GetString(reader.GetOrdinal("Dividend_Type"))

                                };

                                equitySecurityList.Add(equitySecurity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return equitySecurityList;
        }


        //public async Task<List<UpdateEquitySecurity>> GetSecurityDataAsync()
        //{
        //    var securityDataList = new List<UpdateEquitySecurity>();

        //    using (var connection = new SqlConnection(connStr))
        //    {
        //        try
        //        {
        //            // Open the database connection
        //            await connection.OpenAsync();

        //            // Create a SqlDataAdapter to fill a DataSet
        //            using (var adapter = new SqlDataAdapter("Equity.sp_SelectSecurityData", connection))
        //            {
        //                // Specify the command type as stored procedure
        //                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

        //                // Create a DataSet to hold the result
        //                var dataSet = new DataSet();
        //                adapter.Fill(dataSet); // Fill the DataSet with the results of the query

        //                // Assuming the first table in the DataSet contains the desired result
        //                DataTable dataTable = dataSet.Tables[0];

        //                // Iterate through the DataTable rows and map them to your model
        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    var securityData = new UpdateEquitySecurity
        //                    {
        //                        SecurityId = Convert.ToInt32(row["Security_Id"]),
        //                        SecurityName = row.IsNull("Security_Name") ? null : row["Security_Name"].ToString(),
        //                        SecurityDescription = row.IsNull("Security_Description") ? null : row["Security_Description"].ToString(),
        //                        IsActive = row.IsNull("Is_Active") ? (bool?)null : Convert.ToBoolean(row["Is_Active"]),
        //                        PricingCurrency = row.IsNull("Price_Currency") ? null : row["Price_Currency"].ToString(),
        //                        TotalSharesOutstanding = row.IsNull("Shares_Outstanding") ? (decimal?)null : Convert.ToDecimal(row["Shares_Outstanding"]),
        //                        OpenPrice = row.IsNull("Open_Price") ? (decimal?)null : Convert.ToDecimal(row["Open_Price"]),
        //                        ClosePrice = row.IsNull("Close_Price") ? (decimal?)null : Convert.ToDecimal(row["Close_Price"]),
        //                        DividendDeclaredDate = row.IsNull("Dividend_Declared_Date") ? (DateTime?)null : Convert.ToDateTime(row["Dividend_Declared_Date"]),
        //                        PFCreditRating = row.IsNull("PF_Credit_Rating") ? null : row["PF_Credit_Rating"].ToString()
        //                    };

        //                    // Add the mapped model to the list
        //                    securityDataList.Add(securityData);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle any exceptions that may occur during the process
        //            Console.WriteLine($"Error: {ex.Message}");
        //        }
        //    }

        //    return securityDataList;
        //}
    }
}
