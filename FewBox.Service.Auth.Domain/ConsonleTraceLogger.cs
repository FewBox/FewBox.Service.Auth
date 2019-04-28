using System;
using FewBox.Core.Web.Filter;

namespace FewBox.Service.Auth.Domain
{
    public class ConsonleTraceLogger : ITraceLogger
    {
        public void Trace(string name, string param)
        {
            Console.WriteLine($"{name}-{param}");
        }
    }
}