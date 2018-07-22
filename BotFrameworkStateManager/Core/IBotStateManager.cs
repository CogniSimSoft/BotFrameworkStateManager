namespace BotFrameworkStateManager.Core
{
    using System.Collections.Generic;

    public interface IBotStateManager
    {
        IBotState DefaultState { get; set; }
        IBotState CurrentState { get; set; }
        ICollection<IBotState> States { get; set; }

        string QueryState(string query);
    }
}
