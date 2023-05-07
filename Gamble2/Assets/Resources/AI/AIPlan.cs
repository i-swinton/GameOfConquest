using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    using Options;
    public class AIPlan
    {
        List<AIAction> actionSpace;

        List<AIAction> currentPlan;

        int currentIndex;

        Goals.AIGoal goal;

        List<Goals.AIGoal> goals;

        AIPlayer targetPlayer;

        

        public void AddToActionSpace(AIAction action)
        {
            // Adds the action to the search space
            actionSpace.Add(action);
        }

        public void SetGoal(Goals.AIGoal newGoal)
        {
            goal = newGoal;
        }

        public void AddGoal(Goals.AIGoal newGoal)
        {
            goals.Add(newGoal);
        }
        
        public void SetPlayer(AIPlayer player)
        {
            targetPlayer = player;
        }

        public void FormPlan()
        {
            for (int i = 0; i < goals.Count; ++i)
            {
                // If we can find a plan, use that plan
                if(PlanToGoal(out currentPlan, i))
                {
                    return;
                }
            }
        }

        public AIPlan()
        {
            actionSpace = new List<AIAction>();
            goals = new List<Goals.AIGoal>();
        }

        /// <summary>
        /// Performs a planning search on the space. 
        /// </summary>
        /// <param name="plan"></param>
        /// <returns>Returns true if a plan is found. Otherwise, returns false.</returns>
        public bool PlanToGoal(out List<AIAction> plan, int currentGoal)
        {
            // Set the current goL
            goal = goals[currentGoal];

            plan = new List<AIAction>();

            // Perform a dijkstra search 
            List<AIAction> openList = new List<AIAction>();
            List<AIAction> closedList = new List<AIAction>();

            // Find an any action
            for (int i = 0; i < actionSpace.Count; ++i)
            {
                // Find the first any action in the space
                if (actionSpace[i].IsAnyAction|| (AIAction.Match(targetPlayer.WorldState, actionSpace[i])) )
                {
                    openList.Add(actionSpace[i]);
                    break;
                }
            }

            int ittr = 0;
            while (openList.Count > 0)
            {
                // Pop node off of the open list
                AIAction node = CheapestElement(openList, out int index);
                openList.RemoveAt(index);
                // Add to the closed list
                closedList.Add(node);

                #region Return Goal
                // If this node ends at the goal 
                if (AIAction.Match(node, goal.GoalState))
                {
                    ittr = 0;
                    // Recurse the path
                    AIAction current = node;
                    while (current != null)
                    {

                        // Add to the list
                        plan.Add(current);
                        // Recurse
                        current = current.prior;

                        ittr++;
                        if (ittr > actionSpace.Count) { throw new System.Exception("Infinite loop found when recursing path"); }
                    }

                    // Return the plan and report that we have found it
                    return true;
                }
                #endregion

                for (int i = 0; i < actionSpace.Count; ++i)
                {
                    // Skip if on the closed list
                    if (closedList.Contains(actionSpace[i])) { continue; }

                    if (openList.Contains(actionSpace[i]))
                    {
                        // If the actions actually match
                        if (AIAction.Match(node, actionSpace[i]))
                        {
                            if (actionSpace[i].FCost < node.FCost)
                            {
                                // Update the g value
                                actionSpace[i].g = node.g + 1;

                                // Set the parent
                                actionSpace[i].prior = node;
                                // Skip to the next neighbor
                                continue;
                            }
                        }

                        // Other wise continue
                        continue;
                    }

                    // If not on openList or closedList

                    // Check if they are neighbors
                    if (AIAction.Match(node, actionSpace[i]))
                    {
                        // Add to the open list
                        openList.Add(actionSpace[i]);

                        // The action space g
                        actionSpace[i].g = node.g + 1;
                    }


                }
                /// Pop node off top
                /// Is Node the goal
                /// If not, check the neighbors
                /// Update open list


                ittr++;
                if (ittr > 100000)
                {
                    throw new System.Exception("Infinite loop detected");
                }
            }


            return false;
        }

        // Nodes are "connnected" if precondition matches condition? 


        public AIAction CheapestElement(List<AIAction> openList, out int index)
        {
            AIAction cheapest = openList[0];
            index = 0;


            for (int i = 1; i < openList.Count; ++i)
            {
                if (openList[i].FCost <= cheapest.FCost)
                {
                    index = i;
                    cheapest = openList[i];
                }
            }

            return cheapest;
        }

        public void Update(float dt, AIPlayer player)
        {
            // If the current plan does not exist, make one and return
            if(currentPlan == null || currentPlan.Count ==0) { FormPlan(); return; }

            // If the current index is at the end
            if(currentIndex >= currentPlan.Count)
            {

                // Replan
                FormPlan();
                // Reset the index
                currentIndex = 0;

                return;
            }

            // If the current action is completable, continue the plan

            ActionStatus result = currentPlan[currentIndex].PerformAction(player);
            if (result == ActionStatus.Complete)
            {
                ++currentIndex;
            }
            // If the current actions fails, replan
            else if(result == ActionStatus.Failed)
            {

                //PlanToGoal(out currentPlan) ;
                FormPlan();

                currentIndex = 0;
                return;
            }

        }
    }
}
            