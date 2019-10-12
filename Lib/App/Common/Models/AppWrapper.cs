using System;
using App.Common.Interfaces;

namespace App.Common.Models
{
    public class AppWrapper : IAppWrapper
    {
        public AppWrapper()
        {
            Success = true;
        }
        public AppWrapper(Exception ex, string message)
        {
            Success = false;
            Exception = ex;
            Message = message;
        }
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
    public class AppWrapper<T> : AppWrapper, IAppWrapper
    {
        public AppWrapper()
        : base()
        {
            Data = default(T);
        }
        public AppWrapper(T data)
        : base()
        {
            Data = data;
        }
        public AppWrapper(Exception ex, string message)
        : base(ex, message)
        {
            Data = default(T);
        }
        public T Data { get; set; }
    }
}