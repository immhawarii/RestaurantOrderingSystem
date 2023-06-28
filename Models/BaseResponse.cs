namespace RestaurantOrderingSystem.Models
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string ResponseMessage { get; set; }
        public T? Data { get; set; }

        public BaseResponse(int statusCode, string statusMessage, string responseMessage, T? data)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            ResponseMessage = responseMessage;
            Data = data;
        }
    }
}
