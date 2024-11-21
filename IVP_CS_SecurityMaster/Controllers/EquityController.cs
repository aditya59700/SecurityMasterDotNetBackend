using CsvHelper;
using IVP_CS_SecurityMaster.Security_Repository.EquityRepository;
using Microsoft.AspNetCore.Mvc;
using Model_Library;
using Serilog;

namespace IVP_CS_SecurityMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquityController : ControllerBase
    {
        private readonly IEquity _equity;
        private readonly ILogger<EquityController> _logger;

        public EquityController(IEquity equity, ILogger<EquityController> logger)
        {
            _equity = equity;
            _logger = logger;
        }

        [HttpPost("UploadEquityDataFile")]
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

                _equity.ImportDataFromCsv(filePath);

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

                return BadRequest("Invalid CSV format. Please check the column format and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the file.");
                return StatusCode(500, "Internal server error. Please try again later.");
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

        [HttpPut("UpdateEquity/{id}")]
        public async Task<IActionResult> UpdateEquity([FromBody] UpdateEquitySecurity eqm, [FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Updating equity security with ID: {Id}", id);
                var res = await _equity.UpdateSecurityAsync(eqm, id);
                _logger.LogInformation("Equity security updated successfully.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating equity security with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("DeleteEquitySecurity/{id}")]
        public async Task<IActionResult> DeleteEquity([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting equity security with ID: {Id}", id);
                var res = await _equity.DeleteSecurityAsync(id);
                _logger.LogInformation("Equity security deleted successfully.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting equity security with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetAllEquitySecurity")]
        public async Task<IActionResult> GetAllEquity()
        {
            try
            {
                _logger.LogInformation("Fetching all equity securities.");
                var res = await _equity.GetSecurityDataAsync();
                _logger.LogInformation("Successfully fetched all equity securities.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all equity securities.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
