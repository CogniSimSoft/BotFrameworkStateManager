using Microsoft.Bot.Builder.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotFrameworkStateManager.Bot
{
    [Serializable]
    public class BotConversation : BotConversationAdapter, IBotConversation
    {
        public Guid uuid { get; set; }
        public EchoBot Echo { get; set; }
        public BotConversationAdapter Adapter { get; set; }
        public IBotConversationTalkingPoint CurrentTalkingPoint { get; set; }
        public ICollection<IBotConversationTalkingPoint> TalkingPoints { get; set; }
        public IBotConversationTalkingPoint FallbackTalkingPoint { get; set; }

        public async Task Say(string query)
        {
            await this.ProcessActivity(query, 
                async (context) => await Echo.OnTurn(context, Core.Bot.Run(query)));
        }

        public BotConversation(IBotConversationTalkingPoint defaultTalkingPoint, IBotConversationTalkingPoint fallbackTalkingPoint = null)
        {
            this.uuid = Guid.NewGuid();
            
            this.CurrentTalkingPoint = defaultTalkingPoint;
            this.FallbackTalkingPoint = fallbackTalkingPoint ?? defaultTalkingPoint;

            // Create the instance of our Bot.
            Echo = new EchoBot(this);

            base.Use(new ConversationState<EchoState>(new MemoryStorage()));
        }
    }
}
