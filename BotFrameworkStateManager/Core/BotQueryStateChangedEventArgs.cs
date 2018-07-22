namespace BotFrameworkStateManager.Core
{
    using System;

    public class BotQueryStateChangedEventArgs : EventArgs, IBotStateManagerEventArgs
    {
        public IBotState PreviousState { get; set; }
        public IBotState CurrentState { get; set; }
        public string Response { get; set; }
    }
}
