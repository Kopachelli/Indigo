﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndigoEngine.Agents;
using NLog;

namespace IndigoEngine.Actions
{
    [Serializable]
	class ActionGrowFruit : ActionAbstract
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static InfoAboutAction CurrentActionInfo = new InfoAboutAction
																		(
																			new List<Type>()
																			{
																				typeof(AgentTree),
																			},
																			new List<Type>()
																			{
																			},
																			new List<Skill>()
																			{
																			},
																			false,
																			false
																		);

        #region Constructors

			public ActionGrowFruit(Agent argSubj)
			: base(argSubj, null)
			{
				Name = "To grow a fruit on a tree";

				logger.Debug("Created new {0}", this.GetType());
				logger.Trace("Created new {0}", this);
			}

        #endregion	
		
		/// <summary>
		/// ITypicalAction
		/// </summary>
		public override bool CheckForLegitimacy()
		{
			if(!base.CheckForLegitimacy())
			{
				return false;
			}   
			 
			if(!ActionAbstract.CheckForSkills(Subject, CurrentActionInfo.RequiredSkills))
			{
				return false;
			}

            if (Subject.Inventory.ItemList.Count >= Subject.Inventory.StorageSize) //Cheking for item storage errors. May be it is extra checking?
            {
                return false;
            }

			return true;
		}

        /// <summary>
        /// ITypicalAction
        /// </summary>
        public override void CalculateFeedbacks()
        {
            base.CalculateFeedbacks();

            Subject.CurrentActionFeedback += new ActionFeedback(() =>
            {
				//Realisation for tree
				if(Subject is AgentTree)
				{
					Agent newAgentToAdd = new AgentItemFruit();
					newAgentToAdd.Name = "Fruit " + Subject.Inventory.NumberOfAgentsByType(typeof(AgentItemFruit)) + " from " + Subject.Name;
					Subject.Inventory.AddAgentToStorage(newAgentToAdd);

					if(World.AskWorldForAddition(this, newAgentToAdd))
					{
						(Subject as AgentTree).CurrentState.Prolificacy.CurrentPercentValue = 100; 
					}
					else
					{
						Subject.Inventory.PopAgent(newAgentToAdd);
					}
				}
            });

			logger.Info("Action {0}. Fruit grown for {1}", this.Name, Subject.Name);
        }

		/// <summary>
		/// ITypicalAction
		/// </summary>
		public override NameableObject CharacteristicsOfSubject()
		{
			if(Subject is AgentTree)
			{
				return (Subject as AgentTree).CurrentState.Prolificacy; 
			}

			return base.CharacteristicsOfSubject();
		}

		public override int CompareTo(ActionAbstract argActionToCompare)
		{
			return 1; //Action isn't conflict
		}

        public override string ToString()
        {
            return "Action: " + Name;
        }
	}
}
