namespace BotFrameworkStateManager.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IBotMemoryDataModel
    {
        // TODO: Implement "memory" functionality . . . 

        Guid uuid { get; set; }

        string Data { get; set; }
    }
}
