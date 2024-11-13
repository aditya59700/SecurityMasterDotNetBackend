using Microsoft.Data.SqlClient;
using static Model_Library.BondSecurity;
using System.Data;
using System.Globalization;
using Model_Library;

namespace IVP_CS_SecurityMaster.Security_Repository.BondOperation
{
    public class BondOperation : IBond
    {
        string connStr = @"Server = 192.168.0.13\\sqlexpress,49753; Database = IVP_CS_AP; User ID = sa; Password = sa@12345678; TrustServerCertificate = True";

        // Method to import data from a CSV file
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

        // Method to read CSV data into BondSecurity objects
        private List<BondSecurity> ReadCsvFile(string filePath)
        {
            var records = new List<BondSecurity>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<BondSecurity>().ToList();
            }

            return records;
        }

        // Method to insert the full security data and related schedule information
        private void InsertFullSecurityData(BondSecurity data, SqlConnection connection, SqlTransaction transaction)
        {
            int newSecurityID = 0;

            using (SqlCommand command = new SqlCommand("Bond.CreateSecurity", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Main Security Info (Security Summary)
                command.Parameters.AddWithValue("@Security_Name", data.SecurityName ?? (object)DBNull.Value); // Security Name
                command.Parameters.AddWithValue("@Security_Description", data.SecurityDescription ?? (object)DBNull.Value); // Security Description
                command.Parameters.AddWithValue("@Asset_Type", data.AssetType ?? (object)DBNull.Value); // Asset Type
                command.Parameters.AddWithValue("@Investment_Type", data.InvestmentType ?? (object)DBNull.Value); // Investment Type
                command.Parameters.AddWithValue("@Trading_Factor", data.TradingFactor ?? (object)DBNull.Value); // Trading Factor
                command.Parameters.AddWithValue("@Pricing_Factor", data.PricingFactor ?? (object)DBNull.Value); // Pricing Factor
                command.Parameters.AddWithValue("@ISIN", data.ISIN ?? (object)DBNull.Value); // ISIN
                command.Parameters.AddWithValue("@Bloomberg_Ticker", data.BloombergTicker ?? (object)DBNull.Value); // Bloomberg Ticker
                command.Parameters.AddWithValue("@Bloomberg_Unique_Id", data.BloombergUniqueID ?? (object)DBNull.Value); // Bloomberg Unique ID
                command.Parameters.AddWithValue("@CUSIP", data.CUSIP ?? (object)DBNull.Value); // CUSIP
                command.Parameters.AddWithValue("@SEDOL", data.SEDOL ?? (object)DBNull.Value); // SEDOL

                // Bond-specific Fields (Security Details)
                command.Parameters.AddWithValue("@First_Coupon_Date", data.FirstCouponDate ?? (object)DBNull.Value); // First Coupon Date
                command.Parameters.AddWithValue("@Coupon_Cap", data.CouponCap ?? (object)DBNull.Value); // Coupon Cap
                command.Parameters.AddWithValue("@Coupon_Floor", data.CouponFloor ?? (object)DBNull.Value); // Coupon Floor
                command.Parameters.AddWithValue("@Coupon_Frequency", data.CouponFrequency ?? (object)DBNull.Value); // Coupon Frequency
                command.Parameters.AddWithValue("@Coupon_Rate", data.CouponRate ?? (object)DBNull.Value); // Coupon Rate
                command.Parameters.AddWithValue("@Coupon_Type", data.CouponType ?? (object)DBNull.Value); // Coupon Type
                command.Parameters.AddWithValue("@Spread", data.Spread ?? (object)DBNull.Value); // Spread
                command.Parameters.AddWithValue("@Is_Callable", data.IsCallable ?? (object)DBNull.Value); // Is Callable
                command.Parameters.AddWithValue("@Is_Fix_To_Float", data.IsFixToFloat ?? (object)DBNull.Value); // Is Fix To Float
                command.Parameters.AddWithValue("@Is_Putable", data.IsPutable ?? (object)DBNull.Value); // Is Putable
                command.Parameters.AddWithValue("@Issue_Date", data.IssueDate ?? (object)DBNull.Value); // Issue Date
                command.Parameters.AddWithValue("@Last_Reset_Date", data.LastResetDate ?? (object)DBNull.Value); // Last Reset Date
                command.Parameters.AddWithValue("@Maturity_Date", data.MaturityDate ?? (object)DBNull.Value); // Maturity Date
                command.Parameters.AddWithValue("@Max_Call_Notice_Days", data.MaxCallNoticeDays ?? (object)DBNull.Value); // Max Call Notice Days
                command.Parameters.AddWithValue("@Max_Put_Notice_Days", data.MaxPutNoticeDays ?? (object)DBNull.Value); // Max Put Notice Days
                command.Parameters.AddWithValue("@Penultimate_Coupon_Date", data.PenultimateCouponDate ?? (object)DBNull.Value); // Penultimate Coupon Date
                command.Parameters.AddWithValue("@Reset_Frequency", data.ResetFrequency ?? (object)DBNull.Value); // Reset Frequency
                command.Parameters.AddWithValue("@Has_Position", data.HasPosition ?? (object)DBNull.Value); // Has Position Flag

                // Regulatory Details (RegulatoryDetails)
                command.Parameters.AddWithValue("@Form_PF_Asset_Class", data.FormPFAssetClass ?? (object)DBNull.Value); // Asset Class
                command.Parameters.AddWithValue("@Form_PF_Country", data.FormPFCountry ?? (object)DBNull.Value); // Country
                command.Parameters.AddWithValue("@Form_PF_Credit_Rating", data.FormPFCreditRating ?? (object)DBNull.Value); // Credit Rating
                command.Parameters.AddWithValue("@Form_PF_Currency", data.FormPFCurrency ?? (object)DBNull.Value); // Currency
                command.Parameters.AddWithValue("@Form_PF_Instrument", data.FormPFInstrument ?? (object)DBNull.Value); // Instrument
                command.Parameters.AddWithValue("@Form_PF_Liquidity_Profile", data.FormPFLiquidityProfile ?? (object)DBNull.Value); // Liquidity Profile
                command.Parameters.AddWithValue("@Form_PF_Maturity", data.FormPFMaturity ?? (object)DBNull.Value); // Maturity
                command.Parameters.AddWithValue("@Form_PF_NAICS_Code", data.FormPFNAICSCode ?? (object)DBNull.Value); // NAICS Code
                command.Parameters.AddWithValue("@Form_PF_Region", data.FormPFRegion ?? (object)DBNull.Value); // Region
                command.Parameters.AddWithValue("@Form_PF_Sector", data.FormPFSector ?? (object)DBNull.Value); // Sector
                command.Parameters.AddWithValue("@Form_PF_Sub_Asset_Class", data.FormPFSubAssetClass ?? (object)DBNull.Value); // Sub Asset Class

                // Reference Data (ReferenceData)
                command.Parameters.AddWithValue("@Issue_Country", data.IssueCountry ?? (object)DBNull.Value); // Issue Country
                command.Parameters.AddWithValue("@Issue_Currency", data.IssueCurrency ?? (object)DBNull.Value); // Issue Currency
                command.Parameters.AddWithValue("@Issuer", data.Issuer ?? (object)DBNull.Value); // Issuer
                command.Parameters.AddWithValue("@Risk_Currency", data.RiskCurrency ?? (object)DBNull.Value); // Risk Currency

                // Pricing and Analytics (PricingAndAnalytics)
                command.Parameters.AddWithValue("@Ask_Price", data.AskPrice ?? (object)DBNull.Value); // Ask Price
                command.Parameters.AddWithValue("@High_Price", data.HighPrice ?? (object)DBNull.Value); // High Price
                command.Parameters.AddWithValue("@Low_Price", data.LowPrice ?? (object)DBNull.Value); // Low Price
                command.Parameters.AddWithValue("@Open_Price", data.OpenPrice ?? (object)DBNull.Value); // Open Price
                command.Parameters.AddWithValue("@Volume", data.Volume ?? (object)DBNull.Value); // Volume
                command.Parameters.AddWithValue("@Bid_Price", data.BidPrice ?? (object)DBNull.Value); // Bid Price
                command.Parameters.AddWithValue("@Last_Price", data.LastPrice ?? (object)DBNull.Value); // Last Price

                // Insert PutSchedule (Directly as part of same parameters)
                command.Parameters.AddWithValue("@Put_Date", data.PutDate ?? (object)DBNull.Value); // Put Date
                command.Parameters.AddWithValue("@Put_Price", data.PutPrice ?? (object)DBNull.Value); // Put Price

                // Insert CallSchedule (Directly as part of same parameters)
                command.Parameters.AddWithValue("@Call_Date", data.CallDate ?? (object)DBNull.Value); // Call Date
                command.Parameters.AddWithValue("@Call_Price", data.CallPrice ?? (object)DBNull.Value); // Call Price

                // Output parameter to get the New Security ID
                SqlParameter newSecurityIDParam = new SqlParameter("@New_Security_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(newSecurityIDParam);

                // Execute stored procedure to create the bond security
                command.ExecuteNonQuery();

                // Get the new Security ID from the output parameter
                newSecurityID = (int)newSecurityIDParam.Value;

                // Optionally, log or process the newSecurityID here
            }
            Console.WriteLine($"New Security ID: {newSecurityID}");
        }

        public async Task<string> UpdateSecurityAsync(UpdateBondSecurity model)
        {
            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("Bond.sp_UpdateBondData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@security_ID", model.SecurityId);
                        command.Parameters.AddWithValue("@security_description", (object)model.SecurityDescription ?? DBNull.Value);
                        command.Parameters.AddWithValue("@coupon_rate", (object)model.Coupon ?? DBNull.Value);
                        command.Parameters.AddWithValue("@is_callable", (object)model.IsCallable ?? DBNull.Value);
                        command.Parameters.AddWithValue("@penultimate_coupon_date", (object)model.PenultimateCouponDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pf_credit_rating", (object)model.PFCreditRating ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ask_price", (object)model.AskPrice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@bid_price", (object)model.BidPrice ?? DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                        return "Updated Successfully";
                    }
                }
                catch (Exception ex)
                {
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

                    using (var command = new SqlCommand("Bond.DeleteBondSecurity", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Security_ID", securityId);

                        await command.ExecuteNonQueryAsync();
                        return "Deleted Successfully";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "Error";
                }
            }
        }

        public async Task<List<UpdateBondSecurity>> GetSecurityDataAsync()
        {
            var bondDataList = new List<UpdateBondSecurity>();

            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var adapter = new SqlDataAdapter("Bond.sp_SelectBondData", connection))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        var dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        DataTable dataTable = dataSet.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var bondData = new UpdateBondSecurity
                            {
                                SecurityId = Convert.ToInt32(row["SecurityID"]),
                                SecurityDescription = row.IsNull("SecurityDescription") ? null : row["SecurityDescription"].ToString(),
                                Coupon = row.IsNull("CouponRate") ? (decimal?)null : Convert.ToDecimal(row["CouponRate"]),
                                IsCallable = row.IsNull("IsCallable") ? (bool?)null : Convert.ToBoolean(row["IsCallable"]),
                                PenultimateCouponDate = row.IsNull("PenultimateCouponDate") ? (DateTime?)null : Convert.ToDateTime(row["PenultimateCouponDate"]),
                                PFCreditRating = row.IsNull("FormPFCreditRating") ? null : row["FormPFCreditRating"].ToString(),
                                AskPrice = row.IsNull("AskPrice") ? (decimal?)null : Convert.ToDecimal(row["AskPrice"]),
                                BidPrice = row.IsNull("BidPrice") ? (decimal?)null : Convert.ToDecimal(row["BidPrice"])
                            };

                            bondDataList.Add(bondData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return bondDataList;
        }

    }
}
