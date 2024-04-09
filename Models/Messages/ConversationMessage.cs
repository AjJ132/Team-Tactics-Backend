

using System.ComponentModel.DataAnnotations;

namespace TeamTacticsBackend.Models.Messages
{
    public class ConversationMessage
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset SentAt { get; set; }
    }
}
