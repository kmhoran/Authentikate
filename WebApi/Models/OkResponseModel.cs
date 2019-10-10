using System;

namespace WebApi.Models
{
    public class OkResponseModel<T> : ApiResponse<T>
    {
        public OkResponseModel(T data)
        {
            Success = true;
            Data = data;
        }
    }
}