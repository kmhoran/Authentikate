using System;

namespace WebApi.Models
{
    public class NotFoundResponseModel: ApiResponse<string>
    {
        public NotFoundResponseModel(string message)
        {
            Success = false;
            Data = message;
        }
    }
}