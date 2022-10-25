
namespace WebSocketLibrary.Models
{
    public class AcknowledgeMessage: AbstractMessage
    {
        public new string Type { get; set; } = "acknowledge";

        public int AcknowledgingIndex { get; set; }

    }
}
