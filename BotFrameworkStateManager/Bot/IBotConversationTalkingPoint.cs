using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotFrameworkStateManager.Bot
{
    /// <summary>
    /// Topic or Focus Of Conversation
    /// </summary>
    public interface IBotConversationTalkingPoint
    {
        Guid uuid { get; set; }

        string Name { get; set; }
        string Text { get; set; }

        ICollection<IBotConversationTalkingPoint> Transitions { get; set; }
        IDictionary<IBotConversationTalkingPoint, int> TransitionPriorities { get; set; }


        Func<EchoState, IBotConversationTalkingPoint, LuisResult, (bool success, Action<object> callback)> ActivateOn { get; set; }

        bool SetPriority(IBotConversationTalkingPoint talkingPoint, uint priority);
    }
}
