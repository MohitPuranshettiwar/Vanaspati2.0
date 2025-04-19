using Microsoft.AspNetCore.Mvc;

namespace Vanaspati2._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : Controller
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAiVisionService _aiVisionService;
        private readonly IOpenAiService _openAiService;

        public AnalysisController(
            IBlobStorageService blobStorageService,
            IAiVisionService aiVisionService,
            IOpenAiService openAiService)
        {
            _blobStorageService = blobStorageService;
            _aiVisionService = aiVisionService;
            _openAiService = openAiService;
        }

        // Step 3: Create the POST endpoint
        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzePlantImage([FromForm] IFormFile imageFile)
        {
            // Step 4: Validate input
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Invalid image file.");
            }

            try
            {
                // Upload image to Blob Storage
                var imageUrl = await _blobStorageService.UploadImageAsync(imageFile);

                // Analyze image using AI Vision
                var visionResult = await _aiVisionService.AnalyzeImageAsync(imageUrl);

                // Get additional details from OpenAI
                var openAiDetails = await _openAiService.GetDetailsAsync(visionResult.DiseaseName);

                // Populate AnalysisResult
                var analysisResult = new AnalysisResult
                {
                    DiseaseName = visionResult.DiseaseName,
                    Confidence = visionResult.Confidence,
                    Description = openAiDetails.Description,
                    SuggestedTreatment = openAiDetails.SuggestedTreatment,
                    ImageUrl = imageUrl
                };

                // Return success response
                return Ok(analysisResult);
            }
            catch
            {
                // Handle server errors
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
