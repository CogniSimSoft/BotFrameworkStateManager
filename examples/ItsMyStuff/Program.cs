namespace ItsMyStuff
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BotFrameworkStateManager.Bot;
    using Microsoft.Bot.Builder.Luis.Models;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            IBotConversationTalkingPoint defaultTalkingPoint = new BotConversationTalkingPoint();

            // Handle Conversation About An Item
            IBotConversationTalkingPoint askAboutItemTalkingPoint = new BotConversationTalkingPoint();

            // Handle Conversation About An Items' Details.
            IBotConversationTalkingPoint askAboutItemDetailsTalkingPoint = new BotConversationTalkingPoint();

            // Fallback
            IBotConversationTalkingPoint fallBackTalkingPoint = new BotConversationTalkingPoint
            {
                Text = "Sorry, I could not understand."
            };

            // Bot Conversation Handle
            IBotConversation botConversation = new BotConversation(defaultTalkingPoint, fallBackTalkingPoint)
            {
                TalkingPoints = new List<IBotConversationTalkingPoint>() {
                    defaultTalkingPoint,
                    askAboutItemTalkingPoint
                }
            };

            defaultTalkingPoint.Transitions = new List<IBotConversationTalkingPoint>()
            {
                askAboutItemTalkingPoint
            };
            defaultTalkingPoint.TransitionPriorities = new Dictionary<IBotConversationTalkingPoint, int>()
            {
                {askAboutItemTalkingPoint,1}
            };
            defaultTalkingPoint.ActivateOn = new Func<EchoState, IBotConversationTalkingPoint, LuisResult, (bool success, Action<object> callback)>((EchoState state, IBotConversationTalkingPoint contextTalkingPoint, LuisResult luisResult) => {

                return (success: true, callback: null);
            });

            askAboutItemDetailsTalkingPoint.Transitions = new List<IBotConversationTalkingPoint>()
            {
                askAboutItemDetailsTalkingPoint
            };
            askAboutItemDetailsTalkingPoint.ActivateOn = new Func<EchoState, IBotConversationTalkingPoint, LuisResult, (bool success, Action<object> callback)>((EchoState state, IBotConversationTalkingPoint contextTalkingPoint, LuisResult luisResult) =>
            {
                var intentAskAboutItemSize = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Size");
                var intentAskAboutItemColor = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Color");
                var intentAskAboutItemObject = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Item" && entity.Entity != state.ItemContext);

                if (intentAskAboutItemObject != null)
                {
                    if (intentAskAboutItemSize != null)
                    {
                        askAboutItemDetailsTalkingPoint.Text = $"I thought we were talking about the size of my '{state.ItemContext}' ? Ok, let's talk about my '{intentAskAboutItemObject.Entity}'!";
                        
                        return (true, ((object sender) =>
                        {
                            state.ItemContext = intentAskAboutItemObject.Entity;

                        }));
                    }
                    else if (intentAskAboutItemColor != null)
                    {
                        askAboutItemDetailsTalkingPoint.Text = $"I thought we were talking about the color of my '{state.ItemContext}' ? Ok, let's talk about my '{intentAskAboutItemObject.Entity}'!";

                        return (true, ((object sender) =>
                        {
                            state.ItemContext = intentAskAboutItemObject.Entity;

                        }));
                    }
                    else
                    {
                        askAboutItemDetailsTalkingPoint.Text = $"Did you have a question about my '{state.ItemContext}'? (Size/Color)";
                    }

                    //return false;
                }
                else if (intentAskAboutItemSize != null)
                {
                    askAboutItemDetailsTalkingPoint.Text = $"The size of my {state.ItemContext} is small!";
                }

                else if (intentAskAboutItemColor != null)
                {
                    askAboutItemDetailsTalkingPoint.Text = $"The color of my {state.ItemContext} is red!";
                }
                return (success: true, callback: null);
            });

            askAboutItemTalkingPoint.Transitions = new List<IBotConversationTalkingPoint>()
            {
                askAboutItemDetailsTalkingPoint
            };
            askAboutItemTalkingPoint.ActivateOn = new Func<EchoState, IBotConversationTalkingPoint, LuisResult, (bool success, Action<object> callback)>((EchoState state, IBotConversationTalkingPoint contextTalkingPoint, LuisResult luisResult) => {
                var intentAskAboutItem = luisResult.Intents.FirstOrDefault().Intent == "AskAboutItem";
                var intentAskAboutItemSize = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Size");
                var intentAskAboutItemColor = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Color");
                var intentAskAboutItemObject = luisResult.Entities.FirstOrDefault(entity => entity.Type == "MyItems::Item");

                if (intentAskAboutItem && intentAskAboutItemSize != null && intentAskAboutItemObject != null)
                {
                    state.ItemContext = intentAskAboutItemObject.Entity;

                    askAboutItemTalkingPoint.Text = $"The size of my {state.ItemContext} is small!";
                }

                else if (intentAskAboutItem && intentAskAboutItemColor != null && intentAskAboutItemObject != null)
                {
                    state.ItemContext = intentAskAboutItemObject.Entity;

                    askAboutItemTalkingPoint.Text = $"The color of my {state.ItemContext} is red!";
                }

                return (success: true, callback: null);
            });


            // Speak To Bot
            // Context: Cat
            // Response: The color of my cat is red!
            Task botTask = botConversation.Say("What size is your cat?");
            botTask.Wait();

            // Context: Cat
            // Response: The color of my cat is red!
            Task botTask2 = botConversation.Say("What color is he?");
            botTask2.Wait();

            // Context: Cat => Dog
            // Response: I thought we were talking about the color of my 'cat' ? Ok, let's talk about my 'dog'!
            Task botTask3 = botConversation.Say("What color is your dog?");
            botTask3.Wait();

            // Context: Dog
            // Response: The color of my dog is red!
            Task botTask4 = botConversation.Say("What color is he?");
            botTask4.Wait();

            // Context: Dog
            // Response: The color of my dog is red
            // Note: We are still aware that the context is about the dog's size
            Task botTask5 = botConversation.Say("What about the dog?");
            botTask5.Wait();

            // Context: Dog
            // Response: I thought we were talking about the size of my 'dog' ? Ok, let's talk about my 'cat'!
            Task botTask6 = botConversation.Say("What color is your cat?");
            botTask6.Wait();

        }
    }
}