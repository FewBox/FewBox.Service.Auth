using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using System;

namespace FewBox.Service.Auth.ExceptionHandler
{
    public class ConsoleExceptionHandler : IExceptionHandler
    {
        public ErrorResponseDto Handle(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return new ErrorResponseDto(exception.Message);
        }
    }
}