namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public class BotStateChangedEventArgs : EventArgs, IBotStateManagerEventArgs
    {
        public IBotState PreviousState { get; set; }
        public IBotState CurrentState { get; set; }
        public BotStateTransition Transition { get; set; }
        public Dictionary<string, string> EntityContext { get; set; }
    }
}
