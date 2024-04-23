namespace TeamTacticsBackend.DTO.Messages
{
    public class NewConversationDTO
    {
        public string conversationName { get; set; }
        public List<string> userIds { get; set; }
        public bool isGroup { get; set; }
    }

    public class ReturnConversationDTO
    {
        public Guid conversationId { get; set; }
        public string conversationName { get; set; }
        public string? lastMessageSent { get; set; }
        public DateTimeOffset? lastMessageSentTime { get; set; }
        public List<ConversationUserDTO>? users { get; set; } 
    }

    public class ConversationUserDTO
    {
        public Guid? conversationId { get; set; }
        public Guid userId { get; set; }
        public string userName { get; set; }
    }

    public class SendMessageDTO
    {
        public Guid conversationId { get; set; }
        public string message { get; set; }
    }
}
