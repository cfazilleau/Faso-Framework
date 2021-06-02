using System;

using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireLayerAttribute : Attribute
{
	#region Variables

	private LayerMask	layerMask;
	private string		layerName;

	#endregion

	#region Properties

	public LayerMask	LayerMask => layerMask;
	public string		LayerName => layerName;

	#endregion

	#region Methods

	public RequireLayerAttribute(string layer)
	{
		layerMask = LayerMask.NameToLayer(layer);
		layerName = layer;
	}

	public RequireLayerAttribute(LayerMask layer)
	{
		layerMask = layer;
		layerName = LayerMask.LayerToName(layer);
	}

	#endregion
}
