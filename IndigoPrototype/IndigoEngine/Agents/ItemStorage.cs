﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoEngine.Agents
{
	public class ItemStorage : ITypicalItemStorage
	{
		private List<Agent> itemList;  //List of items that are in this storage
		private int storageSize;       //Size of the storage (in number of items)

		#region Constructors
			
		public ItemStorage()
		{
			ItemList = new List<Agent>();
			StorageSize = 0;
		}

		#endregion

		#region Properties 
			
		#region ITypicalItemStorage realisation

		public List<Agent> ItemList
		{
			get	{ return itemList; }
			set	{ itemList = value; }
		}

		public int StorageSize
		{
			get	{ return storageSize; }
			set
			{
				if(value < ItemList.Count)
				{
					throw(new Exception(String.Format("Storage Size of {0} is less than count of objects in it: {1}", this, value)));
				}
				storageSize = value;
			}
		}

		#endregion

		#endregion

		/// <summary>
		/// ITypicalItemStorage
		/// </summary>
		public void AddAgentToStorage(Agent argAgent)
		{
			if(ItemList.Count + 1 > StorageSize)
			{
				throw(new Exception(String.Format("Failed to add {0} to storage {1} cause of lack of space", argAgent, this)));
			}
			argAgent.Location = null;
			ItemList.Add(argAgent);
		}
		
		/// <summary>
		/// ITypicalItemStorage
		/// </summary>
		public Agent GetAgentFromStorage(Agent argAgent)
		{	
			ItemList.Remove(argAgent);
			return argAgent;
		}

		/// <summary>
		/// ITypicalItemStorage
		/// </summary>
		public Agent GetAgentByTypeFromStorage(Type argType)
		{
			return GetAgentFromStorage(ItemList.Find(ag => { return ag.GetType() == argType; }));
		}

		public override string ToString()
		{
			return "Storage: " + itemList.Count.ToString() + "/" + StorageSize.ToString();
		}
	}
}
