using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                //This line creates an instance of the HttpClient class using a factory
                HttpClient client = _httpClientFactory.CreateClient("MangoApi");
                //This line initializes a new instance of the HttpRequestMessage class, which represents an HTTP request.
                //This object will be configured with various parameters before being sent to the API.
                HttpRequestMessage message = new();
                //This line adds an "Accept" header to the HTTP request, specifying that the client expects a JSON/MutiFormData
                //response from the API.
                if(requestDTO.ContentType == ContentType.MultipartFormData)
                {
					message.Headers.Add("Accept", "*/*"); // */* means any media type and any sub type
				}
                else
                    message.Headers.Add("Accept", "application/json");

                //token
                if(withBearer)
                {
                    //Adds an "Authorization" header with a Bearer token obtained from _tokenProvider.
                    //Bearer tokens are commonly used for authentication in API requests.
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");

				}
                
                if(requestDTO.ContentType == ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();
                    foreach(var prop in requestDTO.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDTO.Data);
                        if(value is FormFile)
                        {
                            var file = (FormFile)value;
                            if(file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                            }
                        }
                    }
                }
                else
                {
					if (requestDTO.Data != null)
					{
						message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
					}
				}

                message.RequestUri = new Uri(requestDTO.Url);
                
                HttpResponseMessage? apiResponse = null;

                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    case HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }

        }
    }
}
