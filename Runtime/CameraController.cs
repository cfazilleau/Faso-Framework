using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class CameraController : Controller
{
	#region Variables

	private Camera controlledCamera = null;

	#endregion

	#region Properties

	public Camera ControlledCamera => controlledCamera;

	#endregion

	#region Unity Methods

	protected override void Awake()
	{
		base.Awake();

		controlledCamera = GetComponent<Camera>();
	}

	#endregion
}
