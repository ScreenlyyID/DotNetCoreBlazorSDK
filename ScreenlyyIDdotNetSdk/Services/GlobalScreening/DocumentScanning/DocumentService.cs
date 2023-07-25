using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public class DocumentService : IDocumentService
{
    /// <summary>
        /// Local variables
        /// </summary>
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
        //private readonly string baseUrl = "https://localhost:38418";
        
        private readonly ILogger<DocumentService> _logger;

        /// <summary>
        /// class contructor
        /// </summary>
        /// <param name="httpClient">Http client DI</param>
        public DocumentService(HttpClient httpClient, ILogger<DocumentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        /// <summary>
        /// Getting document instance id
        /// Required when using idv document scans only. Not required for aml, iedv, customer intelligence
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetInstanceId(string correlationId)
        {
            var request = new
            {
                Authenticationsensitivity = 0,
                ClassificationMode = 0,
                Device = new
                {
                    HasContactlessChipReader = false,
                    HasMagneticStripeReader = false,
                    SerialNumber = "JavaScriptWebSDK ",
                    Type = new
                    {
                        Manufacturer = "xxx",
                        Model = "xxx",
                        SensorType = 3,
                    }
                },
                ImageCroppingExpectedSize = 0,
                ImageCroppingMode = 0,
                ManualDocumentType = (object)null,
                ProcessMode = 0,
                SubscriptionId = ""
            };

            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(request);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
                _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

                var response = await _httpClient.PostAsync($"{this.baseUrl}/document/instance", stringContent);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                return string.Empty;
            }
        }


        /// <summary>
        /// Getting document classification
        /// </summary>
        /// <returns></returns>
        public async Task<DocumentClassificationResponse> GetClassification(string instanceId, string correlationId)
        {
            DocumentClassificationResponse result = null;
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
                _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

                var response = await _httpClient.GetAsync($"{baseUrl}/document/{instanceId}/classification");
                var content = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<DocumentClassificationResponse>(content);
               // result = content.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            // return
            return result;
        }


        /// <summary>
        /// Posting image tover
        /// </summary>
        /// <param name="image">Image data</param>
        /// <param name="instance Id">Instance id</param>
        /// <param name="correlation Id">correlationid</param>
        /// <returns></returns>
        public async Task<string> PostDocumentImage(string instanceId, string correlationId, int side, string imageTest)
        {
            var result = "";

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
                _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);


                var sanitizedBase64 = imageTest.Substring(imageTest.LastIndexOf(',') + 1); // image directly from the response.
                byte[] imageData = Convert.FromBase64String(sanitizedBase64);


                var formContent = new MultipartFormDataContent();

                //var test = new StringContent(imageTest);
                var imageContent = new ByteArrayContent(imageData);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

                formContent.Add(imageContent, "file1", "file1");

                //side:
                //0 = front
                //1 = back
                string url = $"{this.baseUrl}/document/{instanceId}/image?side={side}&light=0&metrics=true";
                var response = await _httpClient.PostAsync(url, formContent);
                var content = await response.Content.ReadAsStringAsync();

                result = JsonConvert.SerializeObject(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return result;
        }


        /// <summary>
        /// Get the passport size photo thats used on the ID
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetDocumentImageField(string imageKey, string correlationId, string instanceId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
                _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

                string url = $"{this.baseUrl}/document/{instanceId}/field/image?key={imageKey}";
                var result = await _httpClient.GetStreamAsync(url);

                byte[] bytes;
                await using (var memStream = new MemoryStream())
                {
                    await result.CopyToAsync(memStream);
                    bytes = memStream.ToArray();
                }

                return Convert.ToBase64String(bytes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return string.Empty;
        }

        public async Task<DocumentResponse> GetDocument(string correlationId, string instanceId)
        {
            DocumentResponse result = null;

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
                _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

                string url = $"{this.baseUrl}/document/{instanceId}";
                var response = await _httpClient.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<DocumentResponse>(content); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return result;
        }
    }
