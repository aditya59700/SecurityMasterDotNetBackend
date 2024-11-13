using CsvHelper;
using Microsoft.Data.SqlClient;
using Model_Library;
using System.Data;
using System.Globalization;

namespace IVP_CS_SecurityMaster.Security_Repository.EquityRepository
{
    public class EquityOperation : IEquity
    {
        string connStr = @"Server = 192.168.0.13\\sqlexpress,49753; Database = IVP_CS_AP; User ID = sa; Password = sa@12345678; TrustServerCertificate = True";

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

                        // Commit the transaction if all inserts are successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any insert fails
                        Console.WriteLine("Error during import: " + ex.Message);
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
                records = csv.GetRecords<EquitySecurity>().ToList();
            }

            return records;
        }

        private void InsertFullSecurityData(EquitySecurity data, SqlConnection connection, SqlTransaction transaction)
        {
            int newSecurityID = 0;

            using (SqlCommand command = new SqlCommand("Equity.CreateSecurity", connection))
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


        public async Task<List<UpdateEquitySecurity>> GetSecurityDataAsync()
        {
            var securityDataList = new List<UpdateEquitySecurity>();

            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    // Open the database connection
                    await connection.OpenAsync();

                    // Create a SqlDataAdapter to fill a DataSet
                    using (var adapter = new SqlDataAdapter("Equity.sp_SelectEquitySecurityData", connection))
                    {
                        // Specify the command type as stored procedure
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        // Create a DataSet to hold the result
                        var dataSet = new DataSet();
                        adapter.Fill(dataSet); // Fill the DataSet with the results of the query

                        // Assuming the first table in the DataSet contains the desired result
                        DataTable dataTable = dataSet.Tables[0];

                        // Iterate through the DataTable rows and map them to your model
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var securityData = new UpdateEquitySecurity
                            {
                                SecurityId = Convert.ToInt32(row["Security_Id"]),
                                SecurityName = row.IsNull("Security_Name") ? null : row["Security_Name"].ToString(),
                                SecurityDescription = row.IsNull("Security_Description") ? null : row["Security_Description"].ToString(),
                                IsActive = row.IsNull("Is_Active") ? (bool?)null : Convert.ToBoolean(row["Is_Active"]),
                                PricingCurrency = row.IsNull("Price_Currency") ? null : row["Price_Currency"].ToString(),
                                TotalSharesOutstanding = row.IsNull("Shares_Outstanding") ? (decimal?)null : Convert.ToDecimal(row["Shares_Outstanding"]),
                                OpenPrice = row.IsNull("Open_Price") ? (decimal?)null : Convert.ToDecimal(row["Open_Price"]),
                                ClosePrice = row.IsNull("Close_Price") ? (decimal?)null : Convert.ToDecimal(row["Close_Price"]),
                                DividendDeclaredDate = row.IsNull("Dividend_Declared_Date") ? (DateTime?)null : Convert.ToDateTime(row["Dividend_Declared_Date"]),
                                PFCreditRating = row.IsNull("PF_Credit_Rating") ? null : row["PF_Credit_Rating"].ToString()
                            };

                            // Add the mapped model to the list
                            securityDataList.Add(securityData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during the process
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return securityDataList;
        }
    }
}
