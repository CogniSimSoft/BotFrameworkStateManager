namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;

    public interface IBotStateManager
    {
        IBotState DefaultState { get; set; }
        IBotState CurrentState { get; set; }
        ICollection<IBotState> States { get; set; }

        event EventHandler<IBotStateManagerEventArgs> OnChangingState;
        event EventHandler<IBotStateManagerEventArgs> OnChangedState;

        event EventHandler<IBotStateManagerEventArgs> OnExecutingQuery;
        event EventHandler<IBotStateManagerEventArgs> OnExecutedQuery;

        string QueryState(string query);
    }
}
