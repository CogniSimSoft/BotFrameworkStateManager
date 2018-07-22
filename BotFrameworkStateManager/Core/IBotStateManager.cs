namespace BotFrameworkStateManager.Core
{
    using BotFrameworkStateManager.Core.Memory;
    using System;
    using System.Collections.Generic;

    public interface IBotStateManager
    {
        BotMemory MemoryModel { get; set; }
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
