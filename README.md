# BotFrameworkStateManager
Create complex conversations driven by Cognitive Services and a custom conversational state manager.

The BotFramework StateManager serves as a simple way to implement contextual conversations driven by LUIS and Bot Framework4 using a custom state manager and adapter.

## BotState Transitioning

A real conversation cannot happen without knowing what to respond to someone. IBotConversationTalkingPoint is an interface which holds information pertaining to what the bot should respond to, and how it should respond to it.


-----------

# An example conversation: "It's My Stuff, Ask me about it"

Ask the bot a question about a personal belonging such as 'What color is your hair?' or 'What size is your Yorkie?'

`... setup bot states, bot transitions, etc ...`

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

-----------

# Dependencies : BotFramework v4

* Microsoft.Bot.Builder.CognitiveServices

* Microsoft.Bot.Connector

* Microsoft.Bot.Connector.DirectLine


Documentation coming soon! :)
