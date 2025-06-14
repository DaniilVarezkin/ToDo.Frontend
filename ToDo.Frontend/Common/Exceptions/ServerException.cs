using Microsoft.AspNetCore.Mvc;

namespace ToDo.Frontend.Common.Exceptions
{
    public class ServerException : Exception
    {
        public ProblemDetails? ProblemDetails { get; set; }
        public ServerException(string message, ProblemDetails? problemDetails = null)
            : base(message) 
        {
            ProblemDetails = problemDetails;
        }

    }
}
