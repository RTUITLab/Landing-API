using System;
using System.Collections.Generic;
using System.Text;

namespace Landing.API.Services
{
    public class ParsingException : Exception
    {
        public new ParsingException InnerException { get; }
        public ParsingException(string message, ParsingException innerException) : base(message, innerException)
        {
            InnerException = innerException;
        }
        public ParsingException(string message): base(message)
        {

        }
    }
}
