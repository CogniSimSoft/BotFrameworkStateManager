namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;
    
    public class BotStateTransition
    {
        public Guid uuid { get; set; }
        public ICollection<string[]> RequiresEntities { get; set; }
        public IBotState TransitionTo { get; set; }
        public string Intent { get; set; }

        public BotStateTransition()
        {
            this.uuid = Guid.NewGuid();
        }
    }
}
