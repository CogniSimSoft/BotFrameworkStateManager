namespace ItsMyStuff
{
    using System;
    using BotFrameworkStateManager.Core;
    using System.Collections.Generic;
    using BotFrameworkStateManager.Core.Memory;
    using BotFrameworkStateManager.Memory;
    using Newtonsoft.Json;

    class Program
    {
        public void convoFlow()
        {
            IBotState defaultBotState = new BotState("defaultBotState");
            IBotState confusedBotState = new BotState("confusedBotState")
            {
                ResponseText = "Hmmmm. I didn't understand that",
                Transitions = new List<BotStateTransition>()
                {
                    new BotStateTransition()
                    {
                        //Intent = string.Empty,
                        RequiresEntities = new List<string[]>(){
                        },
                        TransitionTo = defaultBotState
                    },
                },
                CanTransitionAnywhere = false
            };

            //defaultBotState.FallbackTransitionTo = confusedBotState;
            defaultBotState.ResponseText = string.Empty;
            defaultBotState.Transitions = new List<BotStateTransition>()
            {
                new BotStateTransition()
                {
                    //Intent = string.Empty,
                    RequiresEntities = new List<string[]>(){
                        new string[]{"DetailContext", "builtin.keyPhrase" }
                    },
                    TransitionTo = defaultBotState
                }
            };
            defaultBotState.CanTransitionAnywhere = false;


            ICollection<IBotState> botStates = new List<IBotState>()
            {
                // Only Add Default Bot State If You Want ALL Failures To Reset Conversation
                //botDefaultState,
            };

            IBotStateManager stateManager = new BotStateManager(defaultBotState, botStates);

            defaultBotState.OnActivatingState += new EventHandler<IBotStateManagerEventArgs>((object sender, IBotStateManagerEventArgs e) =>
            {
                BotStateChangedEventArgs eventArgs = e as BotStateChangedEventArgs;

                // Is Name Of User Known ?
                if (stateManager.MemoryModel.Neurons.ContainsKey("User::Name") == false)
                {
                    if (eventArgs.EntityContext.ContainsKey("Name"))
                    {
                        IBotMemoryDataModel dataModel = new BotMemoryDataModel<string>();
                        string name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventArgs.EntityContext["Name"]);

                        dataModel.Data = JsonConvert.SerializeObject(name);

                        eventArgs.EntityContext["Name"] = name;

                        e.CurrentState.ResponseText = $"Thanks, {eventArgs.EntityContext["Name"]}! How old are you?";

                        stateManager.MemoryModel.Neurons.Add("User::Name", dataModel);
                    }
                }
                else if (stateManager.MemoryModel.Neurons.ContainsKey("User::Age") == false)
                {
                    if (eventArgs.EntityContext.ContainsKey("builtin.number"))
                    {
                        IBotMemoryDataModel dataModel = new BotMemoryDataModel<string>();
                        int age = Convert.ToInt32(eventArgs.EntityContext["builtin.number"]);

                        dataModel.Data = JsonConvert.SerializeObject(age);

                        e.CurrentState.ResponseText = $"Cool! I am {age + 3} years old! What is your favorite color?";

                        stateManager.MemoryModel.Neurons.Add("User::Age", dataModel);
                    }
                }


                if (stateManager.MemoryModel.Neurons.ContainsKey("User::Name") && stateManager.MemoryModel.Neurons.ContainsKey("User::Age"))
                {
                    e.CurrentState.CanTransitionAnywhere = true;
                }
            });

            confusedBotState.OnActivatedState += new EventHandler<IBotStateManagerEventArgs>((object sender, IBotStateManagerEventArgs e) =>
            {

            });


            // Success => The size of my yorkie is small
            string responseYorkieSize = stateManager.QueryState("My name is Montray.");
            string responseYorkieSize2 = stateManager.QueryState("I like green");
            string responseYorkieSize3 = stateManager.QueryState("I am 10.");
        }

        static void Main(string[] args)
        {

            //convoFlow();

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


            var confusedTrans = new BotStateTransition()

            {

                Intent = "AskAboutItem",

                RequiresEntities = new List<string[]>(){

                            new string[]{"ItemContext", "MyItems::Item"},

                        },

                TransitionTo = botStateConfused

            };
            

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

                    confusedTrans,

                };

            botStateAdditionalInfo.TransitionPriorities = new List<string>() { confusedTrans.uuid.ToString() };
            
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



            IBotStateManager stateManager = new BotStateManager(defaultBotState, botStates);



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