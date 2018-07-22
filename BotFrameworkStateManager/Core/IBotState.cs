namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public interface IBotState
    {
        string BotStateName { get; set; }
        Guid uuid { get; set; }
        Dictionary<string, string> Context { get; set; }
        Dictionary<string, string> ContextMap { get; set; }
        ICollection<IBotState> RelativeStates { get; set; }
        ICollection<BotStateTransition> Transitions { get; set; }
        IList<string> TransitionPriorities { get; set; }
        bool CanTransitionAnywhere { get; set; }
        string ResponseText { get; set; }
        string PrimaryContext { get; set; }
        IBotState ForwardTransition { get; set; }

    }
}
