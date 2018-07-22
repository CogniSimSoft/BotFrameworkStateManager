namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public class BotQueryStateChangedEventArgs : EventArgs, IBotStateManagerEventArgs
    {
        public IBotState PreviousState { get; set; }
        public IBotState CurrentState { get; set; }
        public string Response { get; set; }
        public Dictionary<string, string> EntityContext { get; set; }
    }
}
