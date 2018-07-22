namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public interface IBotStateEvent
    {
        event EventHandler<IBotStateManagerEventArgs> OnActivatingState;
        event EventHandler<IBotStateManagerEventArgs> OnActivatedState;

        void ActivatingState(IBotStateManagerEventArgs e);
        void ActivatedState(IBotStateManagerEventArgs e);
    }
    public class BotStateEvent
    {

        // Bot State Change Event Handlers.
        public event EventHandler<IBotStateManagerEventArgs> OnActivatingState;
        public event EventHandler<IBotStateManagerEventArgs> OnActivatedState;

        public virtual void ActivatingState(IBotStateManagerEventArgs e)
        {
            OnActivatingState?.Invoke(null, e);
        }

        public virtual void ActivatedState(IBotStateManagerEventArgs e)
        {
            OnActivatedState?.Invoke(null, e);
        }
    }

    public class BotState : BotStateEvent, IBotState
    {
        public Guid uuid { get; set; }
        public string BotStateName { get; set; }
        public string ResponseText { get; set; }
        public string PrimaryContext { get; set; }
        public Dictionary<string, string> Context { get; set; }
        public Dictionary<string, string> ContextMap { get; set; }
        //public ICollection<IBotState> RelativeStates { get; set; }
        public ICollection<BotStateTransition> Transitions { get; set; }
        public IList<string> TransitionPriorities { get; set; }
        public IBotState FallbackTransitionTo { get; set; }
        public bool CanTransitionAnywhere { get; set; }




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
