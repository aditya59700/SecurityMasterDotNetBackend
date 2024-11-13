using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_Library
{
    public class UpdateEquitySecurity
    {
        public int SecurityId { get; set; }
        public string SecurityName { get; set; }
        public string SecurityDescription { get; set; }
        public bool? IsActive { get; set; }

        public string PricingCurrency { get; set; }
        public decimal? TotalSharesOutstanding { get; set; }
        public decimal? OpenPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public DateTime? DividendDeclaredDate { get; set; }
        public string PFCreditRating { get; set; }
    }
}
