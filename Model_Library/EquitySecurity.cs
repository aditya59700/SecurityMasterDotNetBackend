using System;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Model_Library
{
 
    public class EquitySecurity
    {
        [Name("Security Name")]
        public string SecurityName { get; set; }

        [Name("Security Description")]
        public string SecurityDescription { get; set; }

        [Name("Has Position")]
        public bool? HasPosition { get; set; }

        [Name("Is Active Security")]
        public bool? IsActive { get; set; }

        [Name("Lot Size")]
        public int? RoundLotSize { get; set; }

        [Name("BBG Unique Name")]
        public string BloombergUniqueName { get; set; }

        // Security Identifiers
        [Name("CUSIP")]
        public string CUSIP { get; set; }

        [Name("ISIN")]
        public string ISIN { get; set; }

        [Name("SEDOL")]
        public string SEDOL { get; set; }

        [Name("Bloomberg Ticker")]
        public string BloombergTicker { get; set; }

        [Name("Bloomberg Unique ID")]
        public string BloombergUniqueID { get; set; }

        [Name("BBG Global ID")]
        public string BloombergGlobalID { get; set; }

        [Name("Ticker and Exchange")]
        public string BloombergTickerAndExchange { get; set; }

        // Security Details
        [Name("Is ADR Flag")]
        public bool? IsADR { get; set; }

        [Name("ADR Underlying Ticker")]
        public string ADRUnderlyingTicker { get; set; }

        [Name("ADR Underlying Currency")]
        public string ADRUnderlyingCurrency { get; set; }

        [Name("Shares Per ADR")]
        public int? SharesPerADR { get; set; }

        [Name("IPO Date")]
        public DateTime? IPODate { get; set; }

        [Name("Pricing Currency")]
        public string PriceCurrency { get; set; }

        [Name("Settle Days")]
        public int? SettleDays { get; set; }

        [Name("Total Shares Outstanding")]
        public decimal? SharesOutstanding { get; set; }

        [Name("Voting Rights Per Share")]
        public decimal? VotingRightsPerShare { get; set; }

        // Risk Details
        [Name("Average Volume - 20D")]
        public decimal? AverageVolume20D { get; set; }

        [Name("Beta")]
        public decimal? Beta { get; set; }

        [Name("Short Interest")]
        public decimal? ShortInterest { get; set; }

        [Name("Return - YTD")]
        public decimal? YTDReturn { get; set; }

        [Name("Volatility - 90D")]
        public decimal? Volatility90D { get; set; }

        // Regulatory Details
        [Name("PF Asset Class")]
        public string PFAssetClass { get; set; }

        [Name("PF Country")]
        public string PFCountry { get; set; }

        [Name("PF Credit Rating")]
        public string PFCreditRating { get; set; }

        [Name("PF Instrument")]
        public string PFInstrument { get; set; }

        [Name("PF Liquidity Profile")]
        public string PFLiquidityProfile { get; set; }

        [Name("PF Maturity")]
        public string PFMaturity { get; set; }

        [Name("PF NAICS Code")]
        public string PFNAICSCode { get; set; }

        [Name("PF Region")]
        public string PFRegion { get; set; }

        [Name("PF Sector")]
        public string PFSector { get; set; }

        [Name("PF Sub Asset Class")]
        public string PFSubAssetClass { get; set; }

        // Reference Data
        [Name("Country of Issuance")]
        public string IssueCountry { get; set; }

        [Name("Exchange")]
        public string Exchange { get; set; }

        [Name("Issuer")]
        public string Issuer { get; set; }

        [Name("Issue Currency")]
        public string IssueCurrency { get; set; }

        [Name("Trading Currency")]
        public string TradingCurrency { get; set; }

        [Name("BBG Industry Sub Group")]
        public string BloombergIndustrySubGroup { get; set; }

        [Name("Bloomberg Industry Group")]
        public string BloombergIndustryGroup { get; set; }

        [Name("Bloomberg Sector")]
        public string BloombergIndustrySector { get; set; }

        [Name("Country of Incorporation")]
        public string CountryOfIncorporation { get; set; }

        [Name("Risk Currency")]
        public string RiskCurrency { get; set; }

        // Pricing Details
        [Name("Open Price")]
        public decimal? OpenPrice { get; set; }

        [Name("Close Price")]
        public decimal? ClosePrice { get; set; }

        [Name("Volume")]
        public decimal? Volume { get; set; }

        [Name("Last Price")]
        public decimal? LastPrice { get; set; }

        [Name("Ask Price")]
        public decimal? AskPrice { get; set; }

        [Name("Bid Price")]
        public decimal? BidPrice { get; set; }

        [Name("PE Ratio")]
        public decimal? PERatio { get; set; }

        // Dividend History
        [Name("Dividend Declared Date")]
        public DateTime? DeclaredDate { get; set; }

        [Name("Dividend Ex Date")]
        public DateTime? ExDate { get; set; }

        [Name("Dividend Record Date ")]
        public DateTime? RecordDate { get; set; }

        [Name("Dividend Pay Date")]
        public DateTime? PayDate { get; set; }

        [Name("Dividend Amount")]
        public decimal? Amount { get; set; }

        [Name("Frequency")]
        public string Frequency { get; set; }

        [Name("Dividend Type")]
        public string Type { get; set; }

        // Output parameter for Security ID
        //public int NewSecurityID { get; set; }
    }
}
