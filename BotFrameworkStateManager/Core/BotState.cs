namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public class BotState : IBotState
    {
        public string BotStateName { get; set; }
        public Guid uuid { get; set; }
        public Dictionary<string, string> Context { get; set; }
        public Dictionary<string, string> ContextMap { get; set; }
        public ICollection<IBotState> RelativeStates { get; set; }
        public ICollection<BotStateTransition> Transitions { get; set; }
        public bool CanTransitionAnywhere { get; set; }
        public string ResponseText { get; set; }
        public string PrimaryContext { get; set; }
        public IBotState ForwardTransition { get; set; }
        public IList<string> TransitionPriorities { get; set; }

        public BotState(string botStateName)
        {
            this.uuid = Guid.NewGuid();
            this.Context = new Dictionary<string, string>();
            this.ContextMap = new Dictionary<string, string>();
            this.Transitions = new List<BotStateTransition>();
            this.TransitionPriorities = new List<string>();

            this.BotStateName = botStateName ?? throw new ArgumentNullException();
        }
    }
}
