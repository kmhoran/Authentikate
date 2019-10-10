using System;

namespace WebApi.Models
{
    public abstract class ApiResponse
    {
        public bool Success { get; protected set; }
    }
    public class ApiResponse<T>: ApiResponse
    {
        public T Data { get; set; }
    }
}