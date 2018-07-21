using System;
using System.Collections.Generic;
using System.Text;

namespace BotFrameworkStateManager.Core
{
    public interface IStateManager
    {
        IBotState DefaultState { get; set; }
        IBotState CurrentState { get; set; }
        ICollection<IBotState> States { get; set; }

        string QueryState(string query);
    }
}
