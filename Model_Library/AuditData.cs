using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_Library
{
    public class AuditData
    {
        public int Audit_ID { get; set; }
        public string Username { get; set; }
        public string SecurityType { get; set; }
        public int Security_ID { get; set; }
        public string Security_Name { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string Column_Name { get; set; }
        public string Old_Value { get; set; }
        public string New_Value { get; set; }
        public string Status { get; set; }
        public string Error_Message { get; set; }
    }
}
