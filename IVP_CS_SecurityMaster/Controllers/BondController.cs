using CsvHelper;
using IVP_CS_SecurityMaster.Security_Repository.BondOperation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model_Library;
using Serilog;

namespace IVP_CS_SecurityMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondController : ControllerBase
    {
        private readonly IBond _bond;
        private readonly ILogger<BondController> _logger;

        public BondController(IBond bond, ILogger<BondController> logger)
        {
            _bond = bond;
            _logger = logger;
        }

        //[HttpPost]
        //public void AddBond()
        //{
        //    _bond.ImportDataFromCsv(@"C:\Users\asparanjape\Downloads\Data for securities.xlsx - Bonds.csv");
        //}

        [HttpPost("UploadBondDataFile")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(file.FileName));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);

                _bond.ImportDataFromCsv(filePath);

                return Ok("File uploaded and data processed successfully.");
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex, "Error reading CSV file.");

                string firstInvalidColumn = GetFirstInvalidColumnFromError(ex.Message);

                if (!string.IsNullOrEmpty(firstInvalidColumn))
                {
                    _logger.LogWarning("Invalid column: {ColumnName}", firstInvalidColumn);
                    return BadRequest($"Invalid column: '{firstInvalidColumn}'. Please check the column format and try again.");
                }

                if (ex.Message.Contains("The conversion cannot be performed"))
                {
                    string invalidColumn = GetInvalidColumnFromConversionError(ex.Message);
                    _logger.LogWarning("Invalid data in column: {ColumnName}", invalidColumn);
                    return BadRequest($"Invalid data in column '{invalidColumn}'. Please ensure the data matches the expected type.");
                }

                return BadRequest("Error processing the CSV file. Please check the file format and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the file.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GetFirstInvalidColumnFromError(string errorMessage)
        {
            var columnPrefix = @"Header with name '";
            var columnSuffix = @"'[0] was not found.";

            if (errorMessage.Contains(columnPrefix) && errorMessage.Contains(columnSuffix))
            {
                var startIndex = errorMessage.IndexOf(columnPrefix) + columnPrefix.Length;
                var endIndex = errorMessage.IndexOf(columnSuffix);

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    _logger.LogInformation("Extracted invalid column: {ColumnName}", errorMessage.Substring(startIndex, endIndex - startIndex));
                    return errorMessage.Substring(startIndex, endIndex - startIndex);
                }
            }

            return null;
        }

        private string GetInvalidColumnFromConversionError(string errorMessage)
        {
            var memberNamePrefix = "MemberName: ";
            var memberNameSuffix = Environment.NewLine;

            if (errorMessage.Contains(memberNamePrefix))
            {
                var startIndex = errorMessage.IndexOf(memberNamePrefix) + memberNamePrefix.Length;
                var endIndex = errorMessage.IndexOf(memberNameSuffix, startIndex);

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    _logger.LogInformation("Extracted invalid column from conversion error: {ColumnName}", errorMessage.Substring(startIndex, endIndex - startIndex).Trim());
                    return errorMessage.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }

            return "Unknown column";
        }

        [HttpPut("UpdateBond/{id}")]
        public async Task<IActionResult> UpdateBond([FromBody] UpdateBondSecurity bondm, [FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Updating bond security with ID: {Id}", id);
                var res = await _bond.UpdateSecurityAsync(bondm, id);
                _logger.LogInformation("Bond security updated successfully.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating bond security with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("DeleteBondSecurity/{id}")]
        public async Task<IActionResult> DeleteBond([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting bond security with ID: {Id}", id);
                var res = await _bond.DeleteSecurityAsync(id);
                _logger.LogInformation("Bond security deleted successfully.");
                return Ok(res);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQl Error occurred while deleting bond security with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting bond security with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetAllBondSecurity")]
        public async Task<IActionResult> GetAllBond()
        {
            try
            {
                _logger.LogInformation("Fetching all bond securities.");
                var res = await _bond.GetSecurityDataAsync();
                _logger.LogInformation("Successfully fetched all bond securities.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all bond securities.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
