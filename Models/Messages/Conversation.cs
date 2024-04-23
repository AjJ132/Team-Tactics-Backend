using System.ComponentModel.DataAnnotations;

namespace TeamTacticsBackend.Models.Messages
{
    public class Conversation
    {
        [Key]
        public Guid ConversationId { get; set; }
        [MaxLength(50)]
        public string ConversationName { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
