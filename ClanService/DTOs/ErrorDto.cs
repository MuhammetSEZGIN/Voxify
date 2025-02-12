using System;

namespace ClanService.DTOs
{
    public class ErrorDto
    {
        /// <summary>
        /// A human-readable error message.
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Detailed error information (optional).
        /// </summary>
        public object Errors { get; set; }
        
        /// <summary>
        /// The time at which the error occurred.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}