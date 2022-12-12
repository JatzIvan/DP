
namespace WebSocketLibrary.Models
{
    public class KeepAliveMessage: AbstractMessage
    {
        public new string Type { get; set; } = "keepalive";

    }
}
