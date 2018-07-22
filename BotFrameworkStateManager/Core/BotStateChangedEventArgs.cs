namespace BotFrameworkStateManager.Core
{
    using System;

    public class BotStateChangedEventArgs : EventArgs, IBotStateManagerEventArgs
    {
        public IBotState PreviousState { get; set; }
        public IBotState CurrentState { get; set; }
        public BotStateTransition Transition { get; set; }
    }
}
