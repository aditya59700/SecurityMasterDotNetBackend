using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Model_Library;
using System.Data;

namespace IVP_CS_SecurityMaster.AuditRepository
{
    public class AuditOperation : IAudit
    {
        string connStr = @"Server = 192.168.0.13\\sqlexpress,49753; Database = IVP_SM_AA; User ID = sa; Password = sa@12345678; TrustServerCertificate = True";


        public async Task<List<AuditData>> GetAuditData()
        {
            var auditRecords = new List<AuditData>();

            using (var connection = new SqlConnection(connStr))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("Master.GetSecurityAuditData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var auditRecord = new AuditData
                                {
                                    Audit_ID = reader.GetInt32(reader.GetOrdinal("Audit_ID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    SecurityType = reader.GetString(reader.GetOrdinal("SecurityType")),
                                    Security_ID = reader.GetInt32(reader.GetOrdinal("Security_ID")),
                                    Security_Name = reader.GetString(reader.GetOrdinal("Security_Name")),
                                    Action = reader.GetString(reader.GetOrdinal("Action")),
                                    Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                                    Column_Name = reader.IsDBNull(reader.GetOrdinal("Column_Name")) ? null : reader.GetString(reader.GetOrdinal("Column_Name")),
                                    Old_Value = reader.IsDBNull(reader.GetOrdinal("Old_Value")) ? null : reader.GetString(reader.GetOrdinal("Old_Value")),
                                    New_Value = reader.IsDBNull(reader.GetOrdinal("New_Value")) ? null : reader.GetString(reader.GetOrdinal("New_Value")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    Error_Message = reader.IsDBNull(reader.GetOrdinal("Error_Message")) ? null : reader.GetString(reader.GetOrdinal("Error_Message"))
                                };
                                auditRecords.Add(auditRecord);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle accordingly
                    throw new Exception("An error occurred while fetching audit data", ex);
                }
            }

            return auditRecords;
        }
    }
}
