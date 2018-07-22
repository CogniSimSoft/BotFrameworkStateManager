namespace BotFrameworkStateManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BotStateManager : IBotStateManager
    {
        public IBotState DefaultState { get; set; }
        public IBotState CurrentState { get; set; }
        public ICollection<IBotState> States { get; set; }

        public BotStateManager(IBotState defaultState, ICollection<IBotState> botStates)
        {
            this.CurrentState = this.DefaultState = defaultState;
            this.States = botStates;
        }

        /// <summary>
        /// Query Luis, Request New Bot State
        /// </summary>
        /// <param name="query">Luis Query</param>
        /// <returns></returns>
        public string QueryState(string query)
        {
            string response = null;
            Microsoft.Bot.Builder.Luis.Models.LuisResult luisResult = BotFrameworkStateManager.Core.Bot.Run(query);

            ICollection<IBotState> fulfilledStates = new List<IBotState>();

            List<BotStateTransition> transitions = this.CurrentState.CanTransitionAnywhere ? this.States.SelectMany((state, nextState) => state.Transitions).Where(trans=>this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())==false).ToList() : this.CurrentState.Transitions.ToList();
            List<BotStateTransition> transitionsWithPriority = this.CurrentState.CanTransitionAnywhere ? this.States.SelectMany((state, nextState) => state.Transitions).Where(trans=>this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())).ToList() : this.CurrentState.Transitions.ToList();

            // Priority items pushed to the bottom of stack.
            transitionsWithPriority.Reverse();

            transitions.InsertRange(transitions.Count, transitionsWithPriority);

            // Run through transitions.
            foreach (BotStateTransition transition in transitions)
            {
                // Entities required for this transition
                IEnumerable<string[]> reqdEntities = transition.RequiresEntities.Select(entity => new string[] { entity[0], entity[1] });

                // Are all entities fulfilled ?
                bool allRequirementsFulfilled = true;

                Dictionary<string, string> tmpDict = new Dictionary<string, string>();

                foreach (string[] entityContext in reqdEntities)
                {
                    string entityTypes = entityContext[1];

                    if (luisResult.Entities.Any(eContext =>
                    {
                        string[] eTypes = entityTypes.Split(";");

                        // Check if EnityRecommendation.Type Equal RequiredEntity.
                        bool hasAnyMatch = eTypes.Any(r =>
                        {
                            string key = r.Split("::").Last().ToLower();

                            if (tmpDict.ContainsKey(entityContext[0]) == false)
                                tmpDict.Add(entityContext[0], key);
                            else
                                tmpDict[entityContext[0]] = key;

                            Microsoft.Bot.Builder.Luis.Models.EntityRecommendation entityWithMatchingType = luisResult.Entities.FirstOrDefault(e => e.Type == r);

                            if (tmpDict.ContainsKey($"*{entityContext[0]}") == false && entityWithMatchingType != null)
                                tmpDict.Add($"*{entityContext[0]}", entityWithMatchingType.Entity);
                            else if(entityWithMatchingType != null)
                                tmpDict[$"*{entityContext[0]}"] = luisResult.Entities.First(e => e.Type == r).Entity;

                            return luisResult.Entities.Any(e => e.Type == r);
                        });

                        return hasAnyMatch;
                    }) == false)
                        allRequirementsFulfilled = false;
                }

                // If Intent set, must equal Luis Response. All required entities must be resolved.
                if (allRequirementsFulfilled && (transition.Intent.Equals(luisResult.Intents.FirstOrDefault()?.Intent, StringComparison.CurrentCultureIgnoreCase) || transition.Intent == null))
                {
                    transition.TransitionTo.ContextMap = tmpDict;

                    fulfilledStates.Add(transition.TransitionTo);

                    bool highestRated = fulfilledStates.OrderBy(fulfillment => fulfillment.Context.Count)?.Last() == transition.TransitionTo;
                    
                    response = transition.TransitionTo.ResponseText;

                    // Get Set Context Values
                    // Replace BotState.Response template with Context Vals
                    foreach (KeyValuePair<string, string> kvp in tmpDict)
                    {
                        if (kvp.Key.IndexOf("*") != 0 && transition.TransitionTo.Context.Count > 0 && response.IndexOf(kvp.Key) >= 0)
                        {
                            response = response.Replace($"[{kvp.Key}]", $"{transition.TransitionTo.Context.First(d => d.Key.Equals(kvp.Value)).Key}", StringComparison.CurrentCultureIgnoreCase);

                            string valRep = transition.TransitionTo.Context.First(d => d.Key.Equals(kvp.Value)).Value;

                            // If *, Get Luis Supplied Entity Value
                            if (valRep == "*" || string.IsNullOrEmpty(valRep) == true)
                            {
                                response = response.Replace($"[{kvp.Key}::Value]", tmpDict[$"*{kvp.Key}"], StringComparison.CurrentCultureIgnoreCase);
                            }

                            response = response.Replace($"[{kvp.Key}::Value]", $"{valRep}", StringComparison.CurrentCultureIgnoreCase);
                        }
                    }

                    if (transition.TransitionTo != null)
                    {
                        this.CurrentState = transition.TransitionTo;
                        Console.WriteLine($"Changed Bot State => {this.CurrentState.BotStateName}");
                    }

                }

            }
            
            if (string.IsNullOrEmpty(response))
                response = "Sorry, I could not understand you!";

            return response;
        }
    }
}
