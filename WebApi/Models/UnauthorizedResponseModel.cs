using System;

namespace WebApi.Models
{
    public class UnauthorizedResponseModel: ApiResponse<string>
    {
        public UnauthorizedResponseModel()
        {
            Success = false;
            Data = "Unauthorized";
        }
    }
}