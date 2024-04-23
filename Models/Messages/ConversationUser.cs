using System.ComponentModel.DataAnnotations;

namespace TeamTacticsBackend.Models.Messages
{
    public class ConversationUser
    {
        [Key]
        public Guid Index { get; set; }
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
    }
}
