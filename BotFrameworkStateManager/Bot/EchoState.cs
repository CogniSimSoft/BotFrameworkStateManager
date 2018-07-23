namespace BotFrameworkStateManager.Bot
{
    using System;

    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
    [Serializable]
    public class EchoState
    {
        public int TurnCount { get; set; } = 0;

        public string ItemContext { get; set; } = null;
    }
}
