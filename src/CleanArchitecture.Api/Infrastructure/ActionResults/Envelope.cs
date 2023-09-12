using System.Diagnostics;
using System.Net;

namespace CleanArchitecture.Api.Infrastructure.ActionResults
{
    public class Envelope
    {
        protected Envelope(int status, string? errorMessage, DateTime timestamp, string? traceId)
        {
            Status = status;
            ErrorMessage = errorMessage;
            Timestamp = timestamp;
            TraceId = traceId;
        }

        protected Envelope()
        {
        }

        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string? TraceId { get; set; }

        public static Envelope Create(string error, HttpStatusCode statusCode)
        {
            return new Envelope((int)statusCode, error, DateTime.UtcNow, Activity.Current?.Id);
        }

        public EnvelopeObjectResult ToActionResult()
        {
            return new EnvelopeObjectResult(this);
        }
    }

    public class CreatedResultEnvelope
    {
        public CreatedResultEnvelope(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}