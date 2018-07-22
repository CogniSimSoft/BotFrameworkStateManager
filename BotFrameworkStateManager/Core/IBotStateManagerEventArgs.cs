using System.Collections.Generic;

namespace BotFrameworkStateManager.Core
{
    public interface IBotStateManagerEventArgs
    {
        IBotState PreviousState { get; set; }
        IBotState CurrentState { get; set; }
        
        Dictionary<string, string> EntityContext { get; set; }
    }
}
