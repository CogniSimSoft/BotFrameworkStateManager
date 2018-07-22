namespace BotFrameworkStateManager.Core.Memory
{
    using BotFrameworkStateManager.Memory;
    using System.Collections.Generic;

    public class BotMemory
    {
        public IDictionary<string, IBotMemoryDataModel> Neurons { get; set; }

        public void Teach(string key, IBotMemoryDataModel neuron)
        {
            if(Neurons.ContainsKey(key)==false)
            {
                Neurons.Add(key, neuron);
            }
            else
            {
                Neurons[key] = neuron;
            }
        }
        public BotMemory()
        {
            this.Neurons = new Dictionary<string, IBotMemoryDataModel>();
        }

    }
}
