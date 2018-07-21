using System;
using System.Collections.Generic;
using System.Text;

namespace BotFrameworkStateManager.Core
{
    public class BotStateTransition
    {
        public ICollection<string[]> RequiresEntities { get; set; }
        public IBotState TransitionTo { get; set; }
        public string Intent { get; set; }
    }
}
