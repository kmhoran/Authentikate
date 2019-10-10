using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class BadRequestResponseModel: ApiResponse<IEnumerable<string>>
    {
        public BadRequestResponseModel(string error): this(new[] {error})
        {}
        public BadRequestResponseModel(IEnumerable<string> errors)
        {
            Success = false;
            Data = errors;
        }
    }
}