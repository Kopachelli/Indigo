﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndigoEngine.Agents;
using System.Drawing;
using NLog;

namespace IndigoEngine.Actions
{
	public static class ActionsManager
	{		
		private static Logger logger = LogManager.GetCurrentClassLogger();

		//Dictionary for different binary actions realisations for different participants
		#region Dictionary itself. Dangerous! Do not open! 

		private static Dictionary<Type, Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>> BinaryActionsDataBase = new Dictionary<Type, Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>>()
		{
			//Action attack dictionary
			{
				typeof(ActionAttack),
				new Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>()
				{
					{
						typeof(AgentLivingIndigo), 
						new Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>()
						{
							{
								typeof(AgentLivingIndigo),
								(sub, ob, par) => {return new ActionAttack(sub, ob, par);}
							}
						}
					}
				}
			},	
			//Action drink dictionary
			{
				typeof(ActionDrink),
				new Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>()
				{
					{
						typeof(AgentLivingIndigo), 
						new Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>()
						{
							{
								typeof(AgentPuddle),
								(sub, ob, par) => {return new ActionDrink(sub, ob);}
							}
						}
					}
				}
			},	
			//Action eat dictionary
			{
				typeof(ActionEat),
				new Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>()
				{
					{
						typeof(AgentLivingIndigo), 
						new Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>()
						{
							{
								typeof(AgentItemFruit),
								(sub, ob, par) => {return new ActionEat(sub, ob);}
							}
						}
					}
				}
			},	
			//Action obtain fruit dictionary
			{
				typeof(ActionObtainFruit),
				new Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>()
				{
					{
						typeof(AgentLivingIndigo), 
						new Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>()
						{
							{
								typeof(AgentTree),
								(sub, ob, par) => {return new ActionObtainFruit(sub, ob);}
							}
						}
					}
				}
			},	
			//Action obtain log dictionary
			{
				typeof(ActionObtainLog),
				new Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>>()
				{
					{
						typeof(AgentLivingIndigo), 
						new Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>()
						{
							{
								typeof(AgentTree),
								(sub, ob, par) => {return new ActionObtainLog(sub, ob);}
							}
						}
					}
				}
			},	
		};

		#endregion
		
		//Dictionary for different witout objects actions realisations for different participants
		#region Dictionary itself. Dangerous! Do not open! 
		private static Dictionary<Type, Dictionary<Type, Func<Agent, object[], ActionAbstract>>> UnaryActionsDataBase = new Dictionary<Type, Dictionary<Type, Func<Agent, object[], ActionAbstract>>>()
		{
			//Action break camp dictionary
			{
				typeof(ActionBreakCamp),
				new Dictionary<Type, Func<Agent, object[], ActionAbstract>>()
				{
					{
						typeof(AgentLivingIndigo), 
						(sub, par) => {return new ActionBreakCamp(sub, par);}
					}
				}
			},
			//Action go dictionary
			{
				typeof(ActionGo),
				new Dictionary<Type, Func<Agent, object[], ActionAbstract>>()
				{
					{
						typeof(AgentLivingIndigo), 
						(sub, par) => {return new ActionGo(sub, par);}
					}
				}
			},
			//Action grow fruit dictionary
			{
				typeof(ActionGrowFruit),
				new Dictionary<Type, Func<Agent, object[], ActionAbstract>>()
				{
					{
						typeof(AgentTree), 
						(sub, par) => {return new ActionGrowFruit(sub);}
					}
				}
			},
			//Action do nothing dictionary
			{
				typeof(ActionDoNothing),
				new Dictionary<Type, Func<Agent, object[], ActionAbstract>>()
				{
					{
						typeof(AgentLivingIndigo), 
						(sub, par) => {return new ActionDoNothing(sub);}
					},
					{
						typeof(AgentCamp), 
						(sub, par) => {return new ActionDoNothing(sub);}
					},
					{
						typeof(AgentItemFruit), 
						(sub, par) => {return new ActionDoNothing(sub);}
					},
					{
						typeof(AgentItemLog), 
						(sub, par) => {return new ActionDoNothing(sub);}
					},
					{
						typeof(AgentPuddle), 
						(sub, par) => {return new ActionDoNothing(sub);}
					},
					{
						typeof(AgentTree), 
						(sub, par) => {return new ActionDoNothing(sub);}
					}
				}
			},
		};	
		#endregion

		public static ActionAbstract GetThisActionForCurrentParticipants(Type argActionType, Agent argSubject, Agent argObject, params object[] argParams)
		{
			logger.Trace("Getting action for action type {0}, subject: {1}, object: {2}", argActionType, argSubject.Name, argObject != null ? argObject.Name : "none");

			if(argActionType.BaseType != typeof(ActionAbstract))
			{
				logger.Error("Trying to get action for none-action type: {0}!", argActionType);
				return null;
			}

			System.Reflection.FieldInfo field = argActionType.GetField("CurrentActionInfo", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			InfoAboutAction currentInfo = field.GetValue(null) as InfoAboutAction;
			
			ActionAbstract result; //Action to return
			if(currentInfo.RequiresObject)
			{
				Dictionary<Type, Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>>> subjectTypeDictionary; //supportive vars
				Dictionary<Type, Func<Agent, Agent, object[], ActionAbstract>> objectTypeDictionary; //supportive vars
				Func<Agent, Agent, object[], ActionAbstract> actionFunc; //supportive vars

				BinaryActionsDataBase.TryGetValue(argActionType, out subjectTypeDictionary);
				subjectTypeDictionary.TryGetValue(argSubject.GetType(), out objectTypeDictionary);
				objectTypeDictionary.TryGetValue(argObject.GetType(), out actionFunc);

				result = actionFunc(argSubject, argObject, argParams);
			}
			else
			{
				Dictionary<Type, Func<Agent, object[], ActionAbstract>> subjectTypeDictionary; //supportive vars
				Func<Agent, object[], ActionAbstract> actionFunc; //supportive vars

				UnaryActionsDataBase.TryGetValue(argActionType, out subjectTypeDictionary);
				subjectTypeDictionary.TryGetValue(argSubject.GetType(), out actionFunc);

				result = actionFunc(argSubject, argParams);				
			}

			return result;
		}
	}
}