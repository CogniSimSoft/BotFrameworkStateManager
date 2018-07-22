namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public interface IBotState : IBotStateEvent
    {
        Guid uuid { get; set; }
        string BotStateName { get; set; }
        string ResponseText { get; set; }
        string PrimaryContext { get; set; }
        Dictionary<string, string> Context { get; set; }
        Dictionary<string, string> ContextMap { get; set; }
        //ICollection<IBotState> RelativeStates { get; set; }
        ICollection<BotStateTransition> Transitions { get; set; }
        IList<string> TransitionPriorities { get; set; }
        IBotState FallbackTransitionTo { get; set; }
        bool CanTransitionAnywhere { get; set; }


        // Bot State Change Event Handlers.
        event EventHandler<IBotStateManagerEventArgs> OnActivatingState;
        event EventHandler<IBotStateManagerEventArgs> OnActivatedState;



    }
}
