using System;

namespace ItsMyStuff
{
    using BotFrameworkStateManager.Core;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    class Program
    {

        static void Main(string[] args)
        {

            IBotState botStateAdditionalInfo = new BotState("botStateAdditionalInfo");
            IBotState botStateConfused = new BotState("botStateConfused");

            IBotState botStateMyItem = new BotState("botStateMyItem")
            {
                PrimaryContext = "ItemContext::item",
                Context = new Dictionary<string, string>()
                {
                    {"item", "*"},
                    {"color", "grey"},
                    {"size", "small"}
                },
                Transitions = new List<BotStateTransition>()
                {
                    new BotStateTransition()
                    {
                        Intent = "AskAboutItem",
                        RequiresEntities = new List<string[]>(){
                            new string[]{"ItemPropertyContext", "MyItems::Color;MyItems::Size" }
                        },
                        TransitionTo = botStateAdditionalInfo
                    },
                },
                CanTransitionAnywhere = false,
                ResponseText = "The [ItemPropertyContext] of my [ItemContext::Value] is [ItemPropertyContext::Value]"
            };

            botStateAdditionalInfo.ForwardTransition = botStateMyItem;
            botStateAdditionalInfo.Context = new Dictionary<string, string>()
                {
                    {"color", "grey"},
                    {"size", "small"}
                };
            botStateAdditionalInfo.CanTransitionAnywhere = false;
            botStateAdditionalInfo.ResponseText = "It is [ItemPropertyContext::Value]";
            botStateAdditionalInfo.Transitions = new List<BotStateTransition>()
                {
                    new BotStateTransition()
                    {
                        Intent = "AskAboutItem",
                        RequiresEntities = new List<string[]>(){
                            new string[]{"ItemPropertyContext", "MyItems::Color;MyItems::Size" }
                        },
                        TransitionTo = botStateAdditionalInfo
                    },
                    new BotStateTransition()
                    {
                        Intent = "AskAboutItem",
                        RequiresEntities = new List<string[]>(){
                            new string[]{"ItemContext", "MyItems::Item"},
                        },
                        TransitionTo = botStateConfused
                    },
                };

            botStateConfused.ForwardTransition = botStateMyItem;
            botStateConfused.Context = new Dictionary<string, string>()
                {
                    {"item", "*"}
                };
            botStateConfused.CanTransitionAnywhere = false;
            botStateConfused.ResponseText = "I'm confused! What were we talking about?";
            botStateConfused.Transitions = new List<BotStateTransition>()
                {
                    new BotStateTransition()
                    {
                        Intent = "AskAboutItem",
                        RequiresEntities = new List<string[]>(){
                            new string[]{"ItemContext", "MyItems::Item"},
                            new string[]{"ItemPropertyContext", "MyItems::Color;MyItems::Size" }
                        },
                        TransitionTo = botStateMyItem
                    },
                };

            IBotState defaultBotState = new BotState("defaultBotState")
            {
                ResponseText = "Welcome!",
                Transitions = new List<BotStateTransition>()
                {
                    new BotStateTransition()
                    {
                        Intent = "AskAboutItem",
                        RequiresEntities = new List<string[]>(){
                            new string[]{"ItemContext", "MyItems::Item"},
                            new string[]{"ItemPropertyContext", "MyItems::Color;MyItems::Size" }
                        },
                        TransitionTo = botStateMyItem
                    },
                },
                CanTransitionAnywhere = true
            };

            ICollection<IBotState> botStates = new List<IBotState>()
            {
                // Only Add Default Bot State If You Want ALL Failures To Reset Conversation
                //botDefaultState,
                botStateAdditionalInfo,
                botStateConfused,
                botStateMyItem
            };

            IStateManager stateManager = new StateManager(defaultBotState, botStates);
            
            // Success => The size of my yorkie is small
            string responseYorkieSize = stateManager.QueryState("What size is your yorkie?");

            // Failure => Context is about yorkie details. Ask about the yorkies' size or color instead!
            // Start Over
            string responseCatColor = stateManager.QueryState("What color is your cat?");

            // Failure => No new context has been set. Conversation was restarted.
            string responseWhatColor = stateManager.QueryState("What color?");

            // Success => The size of my dog is small.
            string responseDogSize = stateManager.QueryState("What size is your dog?");

            // Success => My dog is the color grey
            string responseDogColor = stateManager.QueryState("What color is he?");

            Console.WriteLine("Hello World!");
        }
    }
}