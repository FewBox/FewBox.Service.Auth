using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using System;

namespace FewBox.Service.Auth.Domain
{
    public class ConsoleExceptionHandler : IExceptionHandler
    {
        public ErrorResponseDto Handle(Exception exception)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FewBox: {exception.Message}-{exception.StackTrace}");
            Console.ForegroundColor = consoleColor;
            return new ErrorResponseDto(exception.Message);
        }
    }
}