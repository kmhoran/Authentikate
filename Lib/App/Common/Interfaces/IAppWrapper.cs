using System;

namespace App.Common.Interfaces
{
    public interface IAppWrapper
    {
        bool Success {get; set;}
        Exception Exception {get; set;}
        string Message {get; set;}
    }
}