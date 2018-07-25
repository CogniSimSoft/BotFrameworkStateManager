using BotFrameworkStateManager.Bot;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFrameworkStateManager.Core
{
    public class BotTemplateTalkingPoint
    {
        public string RequiresIntent { get; set; }
        public string[] RequiresEntities { get; set; }
        public string Text { get; set; }
    }

    public class BotTemplate
    {
        public Dictionary<string, BotTemplateTalkingPoint> TalkingPoints { get; set; }

        public string DefaultTalkingPoint { get; set; }
        public string FallbackTalkingPoint { get; set; }

        public Dictionary<string, string[]> Transitions { get; set; }
        public Dictionary<string, Dictionary<string, int>> TransitionPriorities { get; set; }

        public IBotConversation BuildConversation()
        {
            ICollection<IBotConversationTalkingPoint> talkingPoints =
                this.TalkingPoints.Select(talkingPoint => new BotConversationTalkingPoint(talkingPoint.Key) { Text = talkingPoint.Value.Text, ActivateOn = new Func<EchoState, IBotConversationTalkingPoint, Microsoft.Bot.Builder.Luis.Models.LuisResult, (bool success, Action<object> callback)>((EchoState state, IBotConversationTalkingPoint contextTalkingPoint, LuisResult luisResult) =>
                {
                    if(talkingPoint.Value.RequiresIntent != null)
                    {
                        if (luisResult.Intents.FirstOrDefault()?.Intent != talkingPoint.Value.RequiresIntent)
                            return (false, null);
                    }
                    foreach (var entity in talkingPoint.Value.RequiresEntities)
                    {
                        if (luisResult.Entities.Any(e => e.Type == entity) == false)
                            return (false, null);
                    }

                    return (true, null);

                }) }).ToArray();

            foreach(var transition in this.Transitions)
            {
                var tp = talkingPoints.FirstOrDefault(p => p.Name == transition.Key);

                tp.Transitions = talkingPoints.Where(talkingPoint => transition.Value.Contains(talkingPoint.Name)).ToArray();
            }

            foreach (var transitionPriority in this.TransitionPriorities)
            {
                var tp = talkingPoints.FirstOrDefault(p => p.Name == transitionPriority.Key);

                foreach(KeyValuePair<string, int> priority in transitionPriority.Value)
                {
                    tp.TransitionPriorities.Add(talkingPoints.FirstOrDefault(p => p.Name == priority.Key), priority.Value);
                }
            }


            IBotConversationTalkingPoint defaultTalkingPoint = talkingPoints.First(talkingPoint => talkingPoint.Name == this.DefaultTalkingPoint);
            IBotConversationTalkingPoint fallbackTalkingPoint = talkingPoints.First(talkingPoint => talkingPoint.Name == this.FallbackTalkingPoint);

            IBotConversation botConversation = new BotConversation(defaultTalkingPoint, fallbackTalkingPoint);

            return botConversation;
        }

        public BotTemplate()
        {

        }
    }
}
