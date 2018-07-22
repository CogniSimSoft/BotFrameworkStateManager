namespace BotFrameworkStateManager.Core
{
    using BotFrameworkStateManager.Core.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BotStateManager : IBotStateManager
    {
        public BotMemory MemoryModel { get; set; }
        public IBotState DefaultState { get; set; }
        public IBotState CurrentState { get; set; }
        public ICollection<IBotState> States { get; set; }

        // Bot State Change Event Handlers.
        public event EventHandler<IBotStateManagerEventArgs> OnChangingState;
        public event EventHandler<IBotStateManagerEventArgs> OnChangedState;

        // Luis Query Executing Event Handlers.
        public event EventHandler<IBotStateManagerEventArgs> OnExecutingQuery;
        public event EventHandler<IBotStateManagerEventArgs> OnExecutedQuery;

        /// <summary>
        /// Query Luis, Request New Bot State.
        /// </summary>
        /// <param name="query">Luis Query</param>
        /// <returns></returns>
        public string QueryState(string query)
        {

            // Query Executing
            BotQueryStateChangedEventArgs executingQueryArgs = new BotQueryStateChangedEventArgs();
            executingQueryArgs.CurrentState = this.CurrentState;
            executingQueryArgs.PreviousState = null;
            executingQueryArgs.Response = string.Empty;

            OnExecutingQuery?.Invoke(null, executingQueryArgs);


            string response = null;
            Microsoft.Bot.Builder.Luis.Models.LuisResult luisResult = BotFrameworkStateManager.Core.Bot.Run(query);

            ICollection<IBotState> fulfilledStates = new List<IBotState>();

            List<BotStateTransition> transitions = this.CurrentState.CanTransitionAnywhere ? this.States.SelectMany((state, nextState) => state.Transitions).Where(trans=>this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())==false).ToList() : this.CurrentState.Transitions.Where(trans => this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())==false).ToList();
            List<BotStateTransition> transitionsWithPriority = this.CurrentState.CanTransitionAnywhere ? this.States.SelectMany((state, nextState) => state.Transitions).Where(trans=>this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())).ToList() : this.CurrentState.Transitions.Where(trans => this.CurrentState.TransitionPriorities.Contains(trans.uuid.ToString())).ToList();

            // Priority items pushed to the bottom of stack.

            transitions.InsertRange(0, transitionsWithPriority);

            BotStateChangedEventArgs args = new BotStateChangedEventArgs();
            args.CurrentState = this.CurrentState;
            args.PreviousState = null;

            args.EntityContext = luisResult.Entities.Select(e =>
            {
                return new KeyValuePair<string, string>(e.Type, e.Entity);
            }).ToDictionary(d => d.Key, d => d.Value);

            OnChangingState?.Invoke(null, args);

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
                if (allRequirementsFulfilled && (transition.Intent == null || transition.Intent.Equals(luisResult.Intents.FirstOrDefault()?.Intent, StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (transition.TransitionTo != null)
                        transition.TransitionTo.ContextMap = tmpDict;
                    else
                        this.CurrentState.ContextMap = tmpDict;

                    if (transition.TransitionTo != null)
                        fulfilledStates.Add(transition.TransitionTo);

                    this.CurrentState.ActivatingState(args);
                    
                    response = (transition.TransitionTo ?? this.CurrentState).ResponseText;

                    // Get Set Context Values
                    // Replace BotState.Response template with Context Vals
                    foreach (KeyValuePair<string, string> kvp in tmpDict)
                    {
                        if (kvp.Key.IndexOf("*") != 0 && (transition.TransitionTo ?? this.CurrentState).Context.Count > 0 && response.IndexOf(kvp.Key) >= 0)
                        {
                            response = response.Replace($"[{kvp.Key}]", $"{(transition.TransitionTo ?? this.CurrentState).Context.First(d => d.Key.Equals(kvp.Value)).Key}", StringComparison.CurrentCultureIgnoreCase);

                            string valRep = (transition.TransitionTo ?? this.CurrentState).Context.First(d => d.Key.Equals(kvp.Value)).Value;

                            // If *, Get Luis Supplied Entity Value
                            if (valRep == "*" || string.IsNullOrEmpty(valRep) == true)
                            {
                                response = response.Replace($"[{kvp.Key}::Value]", tmpDict[$"*{kvp.Key}"], StringComparison.CurrentCultureIgnoreCase);
                            }

                            response = response.Replace($"[{kvp.Key}::Value]", $"{valRep}", StringComparison.CurrentCultureIgnoreCase);
                        }
                    }

                    this.CurrentState = (transition.TransitionTo ?? this.CurrentState);

                    // Bot State Changed Event
                    BotStateChangedEventArgs stateChangeArgs = new BotStateChangedEventArgs();
                    stateChangeArgs.CurrentState = this.CurrentState;
                    stateChangeArgs.PreviousState = null;
                    stateChangeArgs.Transition = transition;

                    OnChangedState?.Invoke(null, stateChangeArgs);
                    this.CurrentState.ActivatedState(stateChangeArgs);
                        

                    Console.WriteLine($"Changed Bot State => {this.CurrentState.BotStateName}");
                    break;
                }

            }
            
            if (string.IsNullOrEmpty(response))
            {
                if(this.CurrentState.FallbackTransitionTo != null)
                {
                    this.CurrentState = this.CurrentState.FallbackTransitionTo;

                    // Bot State Changed Event
                    BotStateChangedEventArgs stateChangeArgs = new BotStateChangedEventArgs();
                    stateChangeArgs.CurrentState = this.CurrentState;
                    stateChangeArgs.PreviousState = null;
                    stateChangeArgs.Transition = null;

                    this.CurrentState.ActivatedState(stateChangeArgs);
                    return this.CurrentState.ResponseText;
                }
                else
                {
                    return "Sorry, I could not understand your request";
                }
            }
            

            // Query Executed
            BotQueryStateChangedEventArgs executedQueryArgs = new BotQueryStateChangedEventArgs();
            executedQueryArgs.CurrentState = this.CurrentState;
            executedQueryArgs.PreviousState = null;
            executedQueryArgs.Response = response ;

            OnExecutedQuery?.Invoke(null, executedQueryArgs);

            return response;
        }


        public BotStateManager(IBotState defaultState, ICollection<IBotState> botStates)
        {
            this.MemoryModel = new BotMemory();
            this.CurrentState = this.DefaultState = defaultState;
            this.States = botStates;
        }

    }
}
