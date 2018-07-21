# BotFrameworkStateManager
Create complex conversations driven by Cognitive Services and a custom conversational state manager.

The BotFramework StateManager serves as a simple way to implement contextual conversations driven by LUIS using a custom state manager.

## IBotState

The StateManager handles interface IBotState. An IBotState is a simple interface which allows you to configure and handle activities within a state.

## BotStateTransition

A real conversation cannot happen without knowing what to respond to someone. BotStateTransition is a class which holds information pertaining to what the bot should respond to, and how it should respond to it.


-----------

# An example conversation

`... setup bot states, bot transitions, etc ...`

`// Success => The size of my yorkie is small`

`string responseYorkieSize = stateManager.QueryState("What size is your yorkie?");`

User is asking about the size of the bot's Yorkie and responds 'The size of my yorkie is small'.

-----------

`// Failure => Context is about yorkie details. Ask about the yorkies' size or color instead!`

`// Start Over`

`string responseCatColor = stateManager.QueryState("What color is your cat?");`

User is now asking the bot about the color of it's cat. However, the context of the conversation is around the 'Yorkie'. Bot responds with 'I'm confused! What were we talking about?' or a custom message. The conversation is restarted.

-----------

`// Failure => No new context has been set. Conversation was restarted.`

`string responseWhatColor = stateManager.QueryState("What color?");`

User is now asking what color. However, the bot is confused and no longer know we're talking about the Yorkie because you interrupted the context. Ask a question about the dog or some other belonging!

-----------

`// Success => The size of my dog is small.`

`string responseDogSize = stateManager.QueryState("What size is your dog?");`

User now asks question about the size of the dog and responds with 'The size of my dog is small'.

-----------

`// Success => My dog is the color grey`

`string responseHisColor = stateManager.QueryState("What color is he?");`

User now asks what color he is and bot responds with 'It is grey'.

-----------

# Dependencies

* Microsoft.Bot.Builder.CognitiveServices

* Microsoft.Bot.Connector

* Microsoft.Bot.Connector.DirectLine

Documentation coming soon! :)
