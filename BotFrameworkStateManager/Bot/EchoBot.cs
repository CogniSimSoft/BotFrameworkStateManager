namespace BotFrameworkStateManager.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Core.Extensions;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Schema;


    public class EchoBot : IBot
    {
        public IBotConversation Conversation { get; set; }
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context
                var state = context.GetConversationState<EchoState>();

                //// Bump the turn count. 
                //state.TurnCount++;

                // Echo back to the user whatever they typed.
                //await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
                await context.SendActivity($"You sent '{context.Activity.Text}'");
            }
        }

        public async Task OnTurn(ITurnContext context, LuisResult luisResult)
        {
            Action<object> callback = new Action<object>((object sender)=> { });

            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context
                EchoState state = context.GetConversationState<EchoState>();

                // Talking Point Transition Not Set
                // Should Fallback To (*Default) Talking Point
                if(Conversation.CurrentTalkingPoint.Transitions == null)
                {
                    this.Conversation.CurrentTalkingPoint = this.Conversation.FallbackTalkingPoint;
                }
                else
                {
                    // Execute IBotConversationTalkingPoint.ActivateOn()
                    // Allowed Transitions Based On CurrentTalkingPoint and LuisResult
                    IEnumerable<(IBotConversationTalkingPoint talkingPoint, (bool success, Action<object> callback))> canTransitionTo = Conversation.CurrentTalkingPoint.Transitions
                        .Select(talkingPoint =>
                        {
                            return (talkingPoint, talkingPoint.ActivateOn(state, this.Conversation.CurrentTalkingPoint, luisResult));
                        }).Where(transition=>transition.Item2.success);

                    // Check Priorities
                    (IBotConversationTalkingPoint talkingPoint, (bool success, Action<object> callback))[] prioritizedTransitions = canTransitionTo.Where(transition => Conversation.CurrentTalkingPoint.TransitionPriorities.ContainsKey(transition.talkingPoint))
                        .OrderBy(transition=> Conversation.CurrentTalkingPoint.TransitionPriorities[transition.talkingPoint]).ToArray();
                    IEnumerable<(IBotConversationTalkingPoint talkingPoint, (bool success, Action<object> callback))> unPrioritizedTransitions = canTransitionTo.Where(transition => Conversation.CurrentTalkingPoint.TransitionPriorities.ContainsKey(transition.talkingPoint)==false);

                    if(prioritizedTransitions.Count() > 0)
                    {
                        this.Conversation.CurrentTalkingPoint = prioritizedTransitions.First().talkingPoint;

                        prioritizedTransitions.First().Item2.callback?.Invoke(context);
                    }
                    else if(unPrioritizedTransitions.Count() > 0)
                    {
                        this.Conversation.CurrentTalkingPoint = unPrioritizedTransitions.First().talkingPoint;

                        unPrioritizedTransitions.First().Item2.callback?.Invoke(context);
                    }
                    else
                    {
                        this.Conversation.CurrentTalkingPoint = this.Conversation.FallbackTalkingPoint;
                    }
                }

                //// Bump the turn count. 
                //state.TurnCount++;

                // Echo back to the user whatever they typed.
                //await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
                
                await context.SendActivity($"{this.Conversation.CurrentTalkingPoint.Text}");
            }
        }

        public EchoBot(IBotConversation conversation)
        {
            this.Conversation = conversation;
        }
    }
}
