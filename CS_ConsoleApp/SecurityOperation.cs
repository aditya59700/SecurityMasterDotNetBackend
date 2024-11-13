using Microsoft.Data.SqlClient;
using Model_Library;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace CS_ConsoleApp
{
    public class SecurityOperation
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


        //public void ImportSecuritiesFromExcel(string filePath)
        //{
        //    var securities = ReadExcelFile(filePath);
        //    foreach (var security in securities)
        //    {
        //        CreateSecurity(security);
        //    }
        //}

        //private List<EquitySecurity> ReadExcelFile(string filePath)
        //{
        //    var securities = new List<EquitySecurity>();

        //    // Load the Excel package
        //    using (var package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
        //        int rowCount = worksheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++) // Start from row 2, assuming row 1 is the header
        //        {
        //            var security = new EquitySecurity
        //            {
        //                SecurityName = worksheet.Cells[row, 1].Text,
        //                SecurityDescription = worksheet.Cells[row, 2].Text,
        //                HasPosition = ParseBool(worksheet.Cells[row, 3].Text),
        //                IsActive = ParseBool(worksheet.Cells[row, 4].Text),
        //                RoundLotSize = ParseInt(worksheet.Cells[row, 5].Text),
        //                BloombergUniqueName = worksheet.Cells[row, 6].Text,
        //                CUSIP = worksheet.Cells[row, 7].Text,
        //                ISIN = worksheet.Cells[row, 8].Text,
        //                SEDOL = worksheet.Cells[row, 9].Text,
        //                BloombergTicker = worksheet.Cells[row, 10].Text,
        //                BloombergUniqueID = worksheet.Cells[row, 11].Text,
        //                BloombergGlobalID = worksheet.Cells[row, 12].Text,
        //                BloombergTickerAndExchange = worksheet.Cells[row, 13].Text,
        //                IsADR = ParseBool(worksheet.Cells[row, 14].Text),
        //                ADRUnderlyingTicker = worksheet.Cells[row, 15].Text,
        //                ADRUnderlyingCurrency = worksheet.Cells[row, 16].Text,
        //                SharesPerADR = ParseInt(worksheet.Cells[row, 17].Text),
        //                IPODate = ParseDateTime(worksheet.Cells[row, 18].Text),
        //                PriceCurrency = worksheet.Cells[row, 19].Text,
        //                SettleDays = ParseInt(worksheet.Cells[row, 20].Text),
        //                SharesOutstanding = ParseDecimal(worksheet.Cells[row, 21].Text),
        //                VotingRightsPerShare = ParseDecimal(worksheet.Cells[row, 22].Text),
        //                AverageVolume20D = ParseDecimal(worksheet.Cells[row, 23].Text),
        //                Beta = ParseDecimal(worksheet.Cells[row, 24].Text),
        //                ShortInterest = ParseDecimal(worksheet.Cells[row, 25].Text),
        //                YTDReturn = ParseDecimal(worksheet.Cells[row, 26].Text),
        //                Volatility90D = ParseDecimal(worksheet.Cells[row, 27].Text),
        //                PFAssetClass = worksheet.Cells[row, 28].Text,
        //                PFCountry = worksheet.Cells[row, 29].Text,
        //                PFCreditRating = worksheet.Cells[row, 30].Text,
        //                PFInstrument = worksheet.Cells[row, 32].Text,
        //                PFLiquidityProfile = worksheet.Cells[row, 33].Text,
        //                PFMaturity = worksheet.Cells[row, 34].Text,
        //                PFNAICSCode = worksheet.Cells[row, 35].Text,
        //                PFRegion = worksheet.Cells[row, 36].Text,
        //                PFSector = worksheet.Cells[row, 37].Text,
        //                PFSubAssetClass = worksheet.Cells[row, 38].Text,
        //                IssueCountry = worksheet.Cells[row, 39].Text,
        //                Exchange = worksheet.Cells[row, 40].Text,
        //                Issuer = worksheet.Cells[row, 41].Text,
        //                IssueCurrency = worksheet.Cells[row, 42].Text,
        //                TradingCurrency = worksheet.Cells[row, 43].Text,
        //                BloombergIndustrySubGroup = worksheet.Cells[row, 44].Text,
        //                BloombergIndustryGroup = worksheet.Cells[row, 45].Text,
        //                BloombergIndustrySector = worksheet.Cells[row, 46].Text,
        //                CountryOfIncorporation = worksheet.Cells[row, 47].Text,
        //                RiskCurrency = worksheet.Cells[row, 48].Text,
        //                OpenPrice = ParseDecimal(worksheet.Cells[row, 49].Text),
        //                ClosePrice = ParseDecimal(worksheet.Cells[row, 50].Text),
        //                Volume = ParseDecimal(worksheet.Cells[row, 51].Text),
        //                LastPrice = ParseDecimal(worksheet.Cells[row, 52].Text),
        //                AskPrice = ParseDecimal(worksheet.Cells[row, 53].Text),
        //                BidPrice = ParseDecimal(worksheet.Cells[row, 54].Text),
        //                PERatio = ParseDecimal(worksheet.Cells[row, 55].Text),
        //                DeclaredDate = ParseDateTime(worksheet.Cells[row, 56].Text),
        //                ExDate = ParseDateTime(worksheet.Cells[row, 57].Text),
        //                RecordDate = ParseDateTime(worksheet.Cells[row, 58].Text),
        //                PayDate = ParseDateTime(worksheet.Cells[row, 59].Text),
        //                Amount = ParseDecimal(worksheet.Cells[row, 60].Text),
        //                Frequency = worksheet.Cells[row, 61].Text,
        //                DividendType = worksheet.Cells[row, 62].Text
        //            };

        //            securities.Add(security);
        //        }
        //    }

        //    return securities;
        //}

        //private void CreateSecurity(EquitySecurity security)
        //{
        //    using (var connection = new SqlConnection(connStr))
        //    {
        //        connection.Open();

        //        using (var command = new SqlCommand("Equity.CreateSecurity", connection))
        //        {
        //            command.CommandType = System.Data.CommandType.StoredProcedure;

        //            command.Parameters.AddWithValue("@Security_Name", security.SecurityName);
        //            command.Parameters.AddWithValue("@Security_Description", security.SecurityDescription ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Has_Position", security.HasPosition ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Is_Active", security.IsActive ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Round_Lot_Size", security.RoundLotSize ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Unique_Name", security.BloombergUniqueName ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@CUSIP", security.CUSIP ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ISIN", security.ISIN ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@SEDOL", security.SEDOL ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Ticker", security.BloombergTicker ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Unique_ID", security.BloombergUniqueID ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Global_ID", security.BloombergGlobalID ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Ticker_And_Exchange", security.BloombergTickerAndExchange ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Is_ADR", security.IsADR ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ADR_Underlying_Ticker", security.ADRUnderlyingTicker ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ADR_Underlying_Currency", security.ADRUnderlyingCurrency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Shares_Per_ADR", security.SharesPerADR ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@IPO_Date", security.IPODate ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Price_Currency", security.PriceCurrency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Settle_Days", security.SettleDays ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Shares_Outstanding", security.SharesOutstanding ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Voting_Rights_Per_Share", security.VotingRightsPerShare ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Average_Volume_20D", security.AverageVolume20D ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Beta", security.Beta ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Short_Interest", security.ShortInterest ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@YTD_Return", security.YTDReturn ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Volatility_90D", security.Volatility90D ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Asset_Class", security.PFAssetClass ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Country", security.PFCountry ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Credit_Rating", security.PFCreditRating ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Instrument", security.PFInstrument ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Liquidity_Profile", security.PFLiquidityProfile ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Maturity", security.PFMaturity ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_NAICS_Code", security.PFNAICSCode ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Region", security.PFRegion ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Sector", security.PFSector ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PF_Sub_Asset_Class", security.PFSubAssetClass ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Issue_Country", security.IssueCountry ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Exchange", security.Exchange ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Issuer", security.Issuer ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Issue_Currency", security.IssueCurrency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Trading_Currency", security.TradingCurrency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Industry_Sub_Group", security.BloombergIndustrySubGroup ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Industry_Group", security.BloombergIndustryGroup ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bloomberg_Industry_Sector", security.BloombergIndustrySector ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Country_Of_Incorporation", security.CountryOfIncorporation ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Risk_Currency", security.RiskCurrency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Open_Price", security.OpenPrice ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Close_Price", security.ClosePrice ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Volume", security.Volume ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Last_Price", security.LastPrice ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Ask_Price", security.AskPrice ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Bid_Price", security.BidPrice ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@PE_Ratio", security.PERatio ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Declared_Date", security.DeclaredDate ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Ex_Date", security.ExDate ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Record_Date", security.RecordDate ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Pay_Date", security.PayDate ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Amount", security.Amount ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Frequency", security.Frequency ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@Dividend_Type", security.DividendType ?? (object)DBNull.Value);

        //            // Execute the command
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //// Helper methods to parse different types
        //private bool? ParseBool(string value) => bool.TryParse(value, out var result) ? (bool?)result : null;
        //private int? ParseInt(string value) => int.TryParse(value, out var result) ? (int?)result : null;
        //private decimal? ParseDecimal(string value) => decimal.TryParse(value, out var result) ? (decimal?)result : null;
        //private DateTime? ParseDateTime(string value) => DateTime.TryParse(value, out var result) ? (DateTime?)result : null;
    }
}
