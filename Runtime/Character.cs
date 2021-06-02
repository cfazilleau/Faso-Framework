using UnityEngine;

[DefaultExecutionOrder(-90)]
public abstract class Character : MonoBehaviour
{
	#region Variables

	private PlayerController controlledBy = null;

	#endregion

	#region Properties

	public PlayerController ControlledBy => controlledBy;

	#endregion

	#region Unity Methods

	protected virtual void Awake()
	{
		//Default implementation does nothing for now
	}

	protected virtual void Start()
	{
		//Default implementation does nothing for now
	}

	protected virtual void OnDestroy()
	{
		//Default implementation does nothing for now
	}

	protected virtual void Update()
	{
		//Default implementation does nothing for now
	}

	protected virtual void LateUpdate()
	{
		//Default implementation does nothing for now
	}

	protected virtual void FixedUpdate()
	{
		//Default implementation does nothing for now
	}

	#endregion

	#region Custom Methods

	/**
	*	Callback called whenever this character gets possessed by a player controller.
	*/
	public virtual void OnPossessed(PlayerController controller)
	{
		controlledBy = controller;
	}

	/**
	*	Callback called whenever this character gets unpossessed by a player controller.
	*/
	public virtual void OnUnpossessed(PlayerController controller)
	{
		Debug.Assert(controller == controlledBy);

		controlledBy = null;
	}

	#endregion
}
