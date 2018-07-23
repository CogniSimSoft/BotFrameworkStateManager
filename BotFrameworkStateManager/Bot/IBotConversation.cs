using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotFrameworkStateManager.Bot
{
    /// <summary>
    /// Conversation Flow
    /// </summary>
    public interface IBotConversation
    {
        Guid uuid { get; set; }
        EchoBot Echo { get; set; }
        BotConversationAdapter Adapter { get; set; }
        IBotConversationTalkingPoint CurrentTalkingPoint { get; set; }
        ICollection<IBotConversationTalkingPoint> TalkingPoints { get; set; }
        IBotConversationTalkingPoint FallbackTalkingPoint { get; set; }

        Task Say(string query);
    }
}
