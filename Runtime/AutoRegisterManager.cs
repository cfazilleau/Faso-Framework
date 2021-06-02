using System.Collections.Generic;

using UnityEngine;

public class AutoRegisterManager<T> : Manager
{
	#region Variables

	protected readonly HashSet<T> registeredElements = new HashSet<T>();

	#endregion

	#region Properties

	#endregion

	#region Unity Methods

	#endregion

	#region Custom Methods

	public virtual void RegisterElement(T element)
	{
		registeredElements.Add(element);
	}

	public virtual void UnregisterElement(T element)
	{
		registeredElements.Remove(element);
	}

	#endregion
}