namespace BotFrameworkStateManager.Bot
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Luis.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class BotConversationTalkingPoint : IBotConversationTalkingPoint
    {
        public Guid uuid { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }

        public ICollection<IBotConversationTalkingPoint> Transitions { get; set; }
        public IDictionary<IBotConversationTalkingPoint, int> TransitionPriorities { get; set; }

        public Func<EchoState, IBotConversationTalkingPoint, LuisResult, (bool success, Action<object> callback)> ActivateOn { get; set; }

        public bool SetPriority(IBotConversationTalkingPoint talkingPoint, uint priority)
        {
            // -1 Removes Priority
            if (priority < 0)
            {
                this.TransitionPriorities.Remove(talkingPoint);
                return true;
            }
            else if (this.TransitionPriorities.Any(point=>point.Key== talkingPoint))
            {
                if(this.Transitions.Any(point => point == talkingPoint))
                {
                    this.TransitionPriorities[talkingPoint] = (int)priority;
                    return true;
                }
            }
            else
            {
                if(this.Transitions.Any(point => point == talkingPoint))
                {
                    this.TransitionPriorities.Add(talkingPoint, (int)priority);
                }
            }

            return false;
        }

        public BotConversationTalkingPoint(string name = null)
        {
            this.uuid = Guid.NewGuid();
            this.Name = name ?? Guid.NewGuid().ToString();
            this.ActivateOn = ((EchoState state, IBotConversationTalkingPoint contextTalkingPoint, LuisResult luisResult) => { return (false, null); });

            this.Transitions = new List<IBotConversationTalkingPoint>();
            this.TransitionPriorities = new Dictionary<IBotConversationTalkingPoint, int>();
        }
    }
}
