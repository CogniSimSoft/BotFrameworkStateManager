using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotFrameworkStateManager.Core
{
    public class StateManager : IStateManager
    {
        public IBotState DefaultState { get; set; }
        public IBotState CurrentState { get; set; }
        public ICollection<IBotState> States { get; set; }

        public StateManager(IBotState defaultState, ICollection<IBotState> botStates)
        {
            this.CurrentState = this.DefaultState = defaultState;
            this.States = botStates;
        }

        public string QueryState(string query)
        {
            string response = null;
            Microsoft.Bot.Builder.Luis.Models.LuisResult luisResult = BotFrameworkStateManager.Core.Bot.Run(query);

            ICollection<IBotState> fulfilledStates = new List<IBotState>();

            ICollection<BotStateTransition> transitions = this.CurrentState.CanTransitionAnywhere ? this.States.SelectMany((state, nextState) => state.Transitions).ToArray() : this.CurrentState.Transitions;

            foreach (BotStateTransition transition in transitions)
            {
                IEnumerable<string[]> reqdEntities = transition.RequiresEntities.Select(entity => new string[] { entity[0], entity[1] });

                bool allRequirementsFulfilled = true;

                Dictionary<string, string> tmpDict = new Dictionary<string, string>();

                foreach (string[] entityContext in reqdEntities)
                {
                    string entityTypes = entityContext[1];

                    if (luisResult.Entities.Any(eContext =>
                    {
                        string[] eTypes = entityTypes.Split(";");

                        bool hasAnyMatch = eTypes.Any(r =>
                        {
                            string key = r.Split("::").Last().ToLower();

                            if (tmpDict.ContainsKey(entityContext[0]) == false)
                                tmpDict.Add(entityContext[0], key);
                            else
                                tmpDict[entityContext[0]] = key;

                            var entity = luisResult.Entities.FirstOrDefault(e => e.Type == r);

                            if (tmpDict.ContainsKey($"*{entityContext[0]}") == false && entity!=null)
                                tmpDict.Add($"*{entityContext[0]}", entity.Entity);
                            else if(entity != null)
                                tmpDict[$"*{entityContext[0]}"] = luisResult.Entities.First(e => e.Type == r).Entity;

                            bool found = luisResult.Entities.Any(e => e.Type == r);
                            return found;
                        });

                        return hasAnyMatch;
                    }) == false)
                        allRequirementsFulfilled = false;
                }

                if (allRequirementsFulfilled && (transition.Intent.Equals(luisResult.Intents.FirstOrDefault()?.Intent, StringComparison.CurrentCultureIgnoreCase) || transition.Intent == null))
                {
                    transition.TransitionTo.ContextMap = tmpDict;

                    fulfilledStates.Add(transition.TransitionTo);

                    bool highestRated = fulfilledStates.OrderBy(fulfillment => fulfillment.Context.Count)?.Last() == transition.TransitionTo;
                    
                    //if(highestRated)
                    {
                        response = transition.TransitionTo.ResponseText;
                        foreach (KeyValuePair<string, string> kvp in tmpDict)
                        {
                            if (kvp.Key.IndexOf("*") != 0 && transition.TransitionTo.Context.Count > 0 && response.IndexOf(kvp.Key) >= 0)
                            {
                                response = response.Replace($"[{kvp.Key}]", $"{transition.TransitionTo.Context.First(d => d.Key.Equals(kvp.Value)).Key}", StringComparison.CurrentCultureIgnoreCase);

                                string valRep = transition.TransitionTo.Context.First(d => d.Key.Equals(kvp.Value)).Value;

                                if (valRep == "*" || string.IsNullOrEmpty(valRep) == true)
                                {
                                    response = response.Replace($"[{kvp.Key}::Value]", tmpDict[$"*{kvp.Key}"], StringComparison.CurrentCultureIgnoreCase);
                                }
                                response = response.Replace($"[{kvp.Key}::Value]", $"{valRep}", StringComparison.CurrentCultureIgnoreCase);
                            }
                        }
                    }

                }

            }

            //if (fulfilledStates.OrderBy(fulfillment=>fulfillment.Context.Count).ToArray().Count() > 0)
            //    this.CurrentState = fulfilledStates.OrderBy(fulfillment => fulfillment.Context.Count).Last();

            if (fulfilledStates.Count > 0)
            {
                this.CurrentState = fulfilledStates.Last();
                Console.WriteLine($"Changed Bot State => {this.CurrentState.BotStateName}");
            }

            if (string.IsNullOrEmpty(response))
                response = "Sorry, I could not understand you!";

            return response;
        }
    }
}
