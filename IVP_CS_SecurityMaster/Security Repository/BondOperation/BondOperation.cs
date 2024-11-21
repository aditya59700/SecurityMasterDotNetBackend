using Microsoft.Data.SqlClient;
using static Model_Library.BondSecurity;
using System.Data;
using System.Globalization;
using Model_Library;

namespace IVP_CS_SecurityMaster.Security_Repository.BondOperation
{
    public class BondOperation : IBond
    {
        string connStr = @"Server = 192.168.0.13\\sqlexpress,49753; Database = IVP_SM_AA; User ID = sa; Password = sa@12345678; TrustServerCertificate = True";

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

            using (SqlCommand command = new SqlCommand("Master.Insert_Bond_Security", connection, transaction))
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
                command.Parameters.AddWithValue("@Bloomberg_Industry_Group", data.BloombergIndustryGroup ?? (object)DBNull.Value); 
                command.Parameters.AddWithValue("@Bloomberg_Industry_Sub_Group", data.BloombergIndustrySubGroup ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bloomberg_Sector", data.BloombergIndustrySector ?? (object)DBNull.Value); 
                command.Parameters.AddWithValue("@Risk_Currency", data.RiskCurrency ?? (object)DBNull.Value); // Risk Currency

                //Risk
                command.Parameters.AddWithValue("@Duration", data.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Volatility_30D", data.Volatility30D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Volatility_90D", data.Volatility90D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Convexity", data.Convexity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Average_Volume_30D", data.AverageVolume30D ?? (object)DBNull.Value);

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

        public async Task<string> UpdateSecurityAsync(UpdateBondSecurity model,int id)
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
                //try
                //{
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("Bond.DeleteSecurity", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Security_ID", securityId);

                        await command.ExecuteNonQueryAsync();
                        return "Deleted Successfully";
                    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    return "Error";
                //}
            }
        }

        public async Task<List<BondSecurity>> GetSecurityDataAsync()
        {
            var bondDataList = new List<BondSecurity>();

            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var adapter = new SqlDataAdapter("Bond.Select_Bond_Security", connection))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        var dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        DataTable dataTable = dataSet.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var bondData = new BondSecurity
                            {
                                // Security Details
                                SecurityId = Convert.ToInt32(row["Security_Id"]),
                                SecurityName = row.IsNull("Security_Name") ? null : row["Security_Name"].ToString(),
                                SecurityDescription = row.IsNull("Security_Description") ? null : row["Security_Description"].ToString(),
                                AssetType = row.IsNull("Asset_Type") ? null : row["Asset_Type"].ToString(),
                                InvestmentType = row.IsNull("Investment_Type") ? null : row["Investment_Type"].ToString(),
                                TradingFactor = row.IsNull("Trading_Factor") ? (decimal?)null : Convert.ToDecimal(row["Trading_Factor"]),
                                PricingFactor = row.IsNull("Pricing_Factor") ? (decimal?)null : Convert.ToDecimal(row["Pricing_Factor"]),
                                IsActive = row.IsNull("Is_Active") ? (bool?)null : Convert.ToBoolean(row["Is_Active"]),
                                HasPosition = row.IsNull("Has_Position") ? (bool?)null : Convert.ToBoolean(row["Has_Position"]),
                                
                                // Security Identifiers
                                CUSIP = row.IsNull("CUSIP") ? null : row["CUSIP"].ToString(),
                                ISIN = row.IsNull("ISIN") ? null : row["ISIN"].ToString(),
                                SEDOL = row.IsNull("SEDOL") ? null : row["SEDOL"].ToString(),
                                BloombergTicker = row.IsNull("Bloomberg_Ticker") ? null : row["Bloomberg_Ticker"].ToString(),
                                BloombergUniqueID = row.IsNull("Bloomberg_Unique_Id") ? null : row["Bloomberg_Unique_Id"].ToString(),

                                // Bond Details
                                FirstCouponDate = row.IsNull("First_Coupon_Date") ? (DateTime?)null : Convert.ToDateTime(row["First_Coupon_Date"]),
                                CouponCap = row.IsNull("Coupon_Cap") ? (decimal?)null : Convert.ToDecimal(row["Coupon_Cap"]),
                                CouponFloor = row.IsNull("Coupon_Floor") ? (decimal?)null : Convert.ToDecimal(row["Coupon_Floor"]),
                                CouponFrequency = row.IsNull("Coupon_Frequency") ? null : Convert.ToInt32(row["Coupon_Frequency"]),
                                CouponRate = row.IsNull("Coupon_Rate") ? (decimal?)null : Convert.ToDecimal(row["Coupon_Rate"]),
                                CouponType = row.IsNull("Coupon_Type") ? null : row["Coupon_Type"].ToString(),
                                Spread = row.IsNull("Spread") ? (decimal?)null : Convert.ToDecimal(row["Spread"]),
                                IsCallable = row.IsNull("Is_Callable") ? (bool?)null : Convert.ToBoolean(row["Is_Callable"]),
                                IsFixToFloat = row.IsNull("Is_Fix_To_Float") ? (bool?)null : Convert.ToBoolean(row["Is_Fix_To_Float"]),
                                IsPutable = row.IsNull("Is_Putable") ? (bool?)null : Convert.ToBoolean(row["Is_Putable"]),
                                IssueDate = row.IsNull("Issue_Date") ? (DateTime?)null : Convert.ToDateTime(row["Issue_Date"]),
                                LastResetDate = row.IsNull("Last_Reset_Date") ? (DateTime?)null : Convert.ToDateTime(row["Last_Reset_Date"]),
                                MaturityDate = row.IsNull("Maturity_Date") ? (DateTime?)null : Convert.ToDateTime(row["Maturity_Date"]),
                                MaxCallNoticeDays = row.IsNull("Max_Call_Notice_Days") ? (int?)null : Convert.ToInt32(row["Max_Call_Notice_Days"]),
                                MaxPutNoticeDays = row.IsNull("Max_Put_Notice_Days") ? (int?)null : Convert.ToInt32(row["Max_Put_Notice_Days"]),
                                ResetFrequency = row.IsNull("Reset_Frequency") ? null : row["Reset_Frequency"].ToString(),
                                PenultimateCouponDate = row.IsNull("Penultimate_Coupon_Date") ? (DateTime?)null : Convert.ToDateTime(row["Penultimate_Coupon_Date"]),

                                // Bond Risk
                                Duration = row.IsNull("Duration") ? (decimal?)null : Convert.ToDecimal(row["Duration"]),
                                Volatility30D = row.IsNull("Volatility_30D") ? (decimal?)null : Convert.ToDecimal(row["Volatility_30D"]),
                                Volatility90D = row.IsNull("Volatility_90D") ? (decimal?)null : Convert.ToDecimal(row["Volatility_90D"]),
                                Convexity = row.IsNull("Convexity") ? (decimal?)null : Convert.ToDecimal(row["Convexity"]),
                                AverageVolume30D = row.IsNull("Average_Volume_30D") ? (decimal?)null : Convert.ToDecimal(row["Average_Volume_30D"]),

                                // Regulatory Details
                                FormPFAssetClass = row.IsNull("PF_Asset_Class") ? null : row["PF_Asset_Class"].ToString(),
                                FormPFCountry = row.IsNull("PF_Country") ? null : row["PF_Country"].ToString(),
                                FormPFCreditRating = row.IsNull("PF_Credit_Rating") ? null : row["PF_Credit_Rating"].ToString(),
                                FormPFCurrency = row.IsNull("PF_Currency") ? null : row["PF_Currency"].ToString(),
                                FormPFInstrument = row.IsNull("PF_Instrument") ? null : row["PF_Instrument"].ToString(),
                                FormPFLiquidityProfile = row.IsNull("PF_Liquidity_Profile") ? null : row["PF_Liquidity_Profile"].ToString(),
                                FormPFMaturity = row.IsNull("PF_Maturity") ? null : row["PF_Maturity"].ToString(),
                                FormPFNAICSCode = row.IsNull("PF_NAICS_Code") ? null : row["PF_NAICS_Code"].ToString(),
                                FormPFRegion = row.IsNull("PF_Region") ? null : row["PF_Region"].ToString(),
                                FormPFSector = row.IsNull("PF_Sector") ? null : row["PF_Sector"].ToString(),
                                FormPFSubAssetClass = row.IsNull("PF_Sub_Asset_Class") ? null : row["PF_Sub_Asset_Class"].ToString(),

                                // Reference Data
                                IssueCountry = row.IsNull("Issue_Country") ? null : row["Issue_Country"].ToString(),
                                Issuer = row.IsNull("Issuer") ? null : row["Issuer"].ToString(),
                                IssueCurrency = row.IsNull("Issue_Currency") ? null : row["Issue_Currency"].ToString(),
                                BloombergIndustrySubGroup = row.IsNull("Bloomberg_Industry_Sub_Group") ? null : row["Bloomberg_Industry_Sub_Group"].ToString(),
                                BloombergIndustryGroup = row.IsNull("Bloomberg_Industry_Group") ? null : row["Bloomberg_Industry_Group"].ToString(),
                                BloombergIndustrySector = row.IsNull("Bloomberg_Industry_Sector") ? null : row["Bloomberg_Industry_Sector"].ToString(),
                                RiskCurrency = row.IsNull("Risk_Currency") ? null : row["Risk_Currency"].ToString(),

                                // Pricing Details
                                OpenPrice = row.IsNull("Open_Price") ? (decimal?)null : Convert.ToDecimal(row["Open_Price"]),
                                Volume = row.IsNull("Volume") ? (decimal?)null : Convert.ToDecimal(row["Volume"]),
                                LastPrice = row.IsNull("Last_Price") ? (decimal?)null : Convert.ToDecimal(row["Last_Price"]),
                                AskPrice = row.IsNull("Ask_Price") ? (decimal?)null : Convert.ToDecimal(row["Ask_Price"]),
                                BidPrice = row.IsNull("Bid_Price") ? (decimal?)null : Convert.ToDecimal(row["Bid_Price"]),
                                HighPrice = row.IsNull("High_Price") ? (decimal?)null : Convert.ToDecimal(row["High_Price"]),
                                LowPrice = row.IsNull("Low_Price") ? (decimal?)null : Convert.ToDecimal(row["Low_Price"]),

                                // Call and Put Schedules
                                CallDate = row.IsNull("Call_Date") ? (DateTime?)null : Convert.ToDateTime(row["Call_Date"]),
                                CallPrice = row.IsNull("Call_Price") ? (decimal?)null : Convert.ToDecimal(row["Call_Price"]),
                                PutDate = row.IsNull("Put_Date") ? (DateTime?)null : Convert.ToDateTime(row["Put_Date"]),
                                PutPrice = row.IsNull("Put_Price") ? (decimal?)null : Convert.ToDecimal(row["Put_Price"])
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


        //public async Task<List<UpdateBondSecurity>> GetSecurityDataAsync()
        //{
        //    var bondDataList = new List<UpdateBondSecurity>();

        //    using (var connection = new SqlConnection(connStr))
        //    {
        //        try
        //        {
        //            await connection.OpenAsync();

        //            using (var adapter = new SqlDataAdapter("Bond.sp_SelectBondData", connection))
        //            {
        //                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

        //                var dataSet = new DataSet();
        //                adapter.Fill(dataSet);

        //                DataTable dataTable = dataSet.Tables[0];

        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    var bondData = new UpdateBondSecurity
        //                    {
        //                        SecurityId = Convert.ToInt32(row["SecurityID"]),
        //                        SecurityName = row.IsNull("SecurityName") ? null : row["SecurityName"].ToString(),
        //                        SecurityDescription = row.IsNull("SecurityDescription") ? null : row["SecurityDescription"].ToString(),
        //                        IsActive = row.IsNull("Is_Active") ? (bool?)null : Convert.ToBoolean(row["Is_Active"]),
        //                        Coupon = row.IsNull("CouponRate") ? (decimal?)null : Convert.ToDecimal(row["CouponRate"]),
        //                        IsCallable = row.IsNull("IsCallable") ? (bool?)null : Convert.ToBoolean(row["IsCallable"]),
        //                        MaturityDate = row.IsNull("MaturityDate") ? (DateTime?)null : Convert.ToDateTime(row["MaturityDate"]),
        //                        PenultimateCouponDate = row.IsNull("PenultimateCouponDate") ? (DateTime?)null : Convert.ToDateTime(row["PenultimateCouponDate"]),
        //                        PFCreditRating = row.IsNull("FormPFCreditRating") ? null : row["FormPFCreditRating"].ToString(),
        //                        AskPrice = row.IsNull("AskPrice") ? (decimal?)null : Convert.ToDecimal(row["AskPrice"]),
        //                        BidPrice = row.IsNull("BidPrice") ? (decimal?)null : Convert.ToDecimal(row["BidPrice"])
        //                    };

        //                    bondDataList.Add(bondData);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error: {ex.Message}");
        //        }
        //    }

        //    return bondDataList;
        //}

    }
}
