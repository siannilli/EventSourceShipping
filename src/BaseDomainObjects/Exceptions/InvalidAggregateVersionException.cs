using System;

namespace BaseDomainObjects.Exceptions
{   
    class InvalidAggregateVersionException : Exception
    {
        public InvalidAggregateVersionException()
        {
        }

        public InvalidAggregateVersionException(string message) : base(message)
        {
        }

        public InvalidAggregateVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}