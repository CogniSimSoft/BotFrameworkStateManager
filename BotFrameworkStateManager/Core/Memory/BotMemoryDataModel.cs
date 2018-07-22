namespace BotFrameworkStateManager.Core.Memory
{
    using BotFrameworkStateManager.Memory;
    using Newtonsoft.Json;
    using System;

    public class BotMemoryDataModel<T> : IBotMemoryDataModel
    {
        public Guid uuid { get; set; }
        public string Data { get; set; }

        public T Model => JsonConvert.DeserializeObject<T>(this.Data);
    }
}
