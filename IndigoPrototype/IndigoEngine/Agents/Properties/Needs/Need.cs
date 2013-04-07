﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoEngine
{
    /// <summary>
    /// Class of need
    /// </summary>
    public class Need : NameableObject, ITypicalNeed
    {
        int needLevel;                    //Need level like in Maslow's hierarchy of needs 
        int needSubLevel;                 //Sublevel to more flexible model
        List<Action> satisfyingActions; //List of actions, that can satisfy the need

        #region Constructors

		/// <summary>
		/// Empty constructor, fills the class with default values
		/// </summary>
        public Need()
        {
            needLevel = 10;
            needSubLevel = 1;
            satisfyingActions = new List<Action>();
        }

        /// <summary>
        /// Need constuctor from one action
        /// </summary>
        /// <param name="argName">name of need</param>
        /// <param name="argNeedLevel">need level like in Maslow's hierarchy of needs </param>
        /// <param name="argNeedSubLevel">sublevel to more flexible model</param>
        /// <param name="argSatisfyingActionID"> action, that satisfy this need</param>
        public Need(string argName, int argNeedLevel, int argNeedSubLevel, Action argSatisfyingActionID)
        {
			Name = argName;
            NeedLevel = argNeedLevel;
            NeedSubLevel = argNeedSubLevel;
            SatisfyingActions = new List<Action>();
            SatisfyingActions.Add(argSatisfyingActionID);
        }

        /// <summary>
        /// Need constuctor from List of action
        /// </summary>
        /// <param name="argName">name of need</param>
        /// <param name="argNeedLevel">need level like in Maslow's hierarchy of needs</param>
        /// <param name="argNeedSubLevel">sublevel to more flexible model</param>
        /// <param name="argSatisfyingActionIDs">actions, that satisfy this need</param>
        public Need(string argName, int argNeedLevel, int argNeedSubLevel, List<Action> argSatisfyingActionIDs)
        {
            Name = argName;
            NeedLevel = argNeedLevel;
            NeedSubLevel = argNeedSubLevel;
            SatisfyingActions = argSatisfyingActionIDs;
        }

		/// <summary>
		/// Need constuctor without need sublevel
		/// </summary>
		/// <param name="argName">name of need</param>
		/// <param name="argNeedLevel">need level like in Maslow's hierarchy of needs</param>
		/// <param name="argSatisfyingActionIDs">actions, that satisfy this need</param>
        public Need(string argName, int argNeedLevel, List<Action> argSatisfyingActionIDs) 
			: this(argName, argNeedLevel, 0, argSatisfyingActionIDs)
		{
		}

		#endregion

		#region ITypicalNeed realisation

        public int NeedLevel
        {
            get { return needLevel; }
			set { needLevel = value; }
        }

        public int NeedSubLevel
        {
            get { return needSubLevel; }
			set { needSubLevel = value; }
        }

        public List<Action> SatisfyingActions
        {
            get { return satisfyingActions; }
			set { satisfyingActions = value; }
        }

        #endregion

        public override string ToString()
        {
            return Name + ", lvl " + NeedLevel.ToString() + "," + NeedSubLevel.ToString();
        }
    }
}