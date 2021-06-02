using System;
using System.Reflection;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
public abstract class UIPanel : MonoBehaviour
{
	#region Variables

	[Header("Canvas Settings"), InfoBox("This component is controlling the attached Canvas and CanvasGroup")]
	[SerializeField, Tooltip("Is Canvas enabled or disabled on awake")]
	private bool enabledOnAwake = false;

	[Header("Panel Settings")]
	[SerializeField]
	private bool forwardInputsOnShow = true;
	
	public event Action onShow	= null;
	public event Action onHide	= null;

	private bool interactable = true;
	private bool blockRaycasts = true;
	private bool ignoreParentgroups = false;

	private bool forwardInput = false;

	protected Canvas canvas = null;
	protected CanvasGroup canvasGroup = null;
	protected UIController uiController = null;

	private BindingFlags	methodFlags				= BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#if !UNITY_EDITOR
	private Type[]			methodParameterTypes	= new Type[] { typeof(InputAction.CallbackContext) };
#endif
	private object[]		parameters				= new object[1];

	#endregion

	#region Properties

	public RectTransform RectTransform => transform as RectTransform;

	public Canvas Canvas => canvas;

	public bool VisibleOnAwake => enabledOnAwake;

	public bool ForwardInput
	{
		get => forwardInput;
		set => SetForwardInput(value);
	}

	public bool Visible
	{
		get => canvas.enabled;
		protected set => canvas.enabled = value;
	}

	public bool Interactable
	{
		get => canvasGroup.interactable;
		protected set => canvasGroup.interactable = value;
	}

	public bool BlocksRaycasts
	{
		get => canvasGroup.blocksRaycasts;
		protected set => canvasGroup.blocksRaycasts = value;
	}

	public bool IgnoreParentGroups
	{
		get => canvasGroup.ignoreParentGroups;
		protected set => canvasGroup.ignoreParentGroups = value;
	}

	public float Alpha
	{
		get => canvasGroup.alpha;
		set => canvasGroup.alpha = value;
	}

	#endregion

	#region Unity Methods

	protected virtual void Awake()
	{
		// Get References
		canvas		= GetComponent<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();
		
		uiController = GameInstance.GameflowManager.UIController;

		if (canvasGroup)
		{
			// Get base states
			interactable		= canvasGroup.interactable;
			blockRaycasts		= canvasGroup.blocksRaycasts;
			ignoreParentgroups	= canvasGroup.ignoreParentGroups;

			// Init visibility
			if (enabledOnAwake)
			{
				canvasGroup.interactable		= interactable;
				canvasGroup.blocksRaycasts		= blockRaycasts;
				canvasGroup.ignoreParentGroups	= ignoreParentgroups;
			}
			else
			{
				canvasGroup.interactable		= false;
				canvasGroup.blocksRaycasts		= false;
				canvasGroup.ignoreParentGroups	= true;
			}
		}

		// Init canvas visibility
		canvas.enabled = enabledOnAwake;

		if (enabledOnAwake && forwardInputsOnShow)
			ForwardInput = true;
	}

	#endregion

	#region Custom Methods

	public void Show()
	{
		if (!Visible)
			OnShow();
	}

	public void Hide()
	{
		if (Visible)
			OnHide();
	}

	protected virtual void OnShow()
	{
		canvas.enabled = true;

		if (canvasGroup)
		{
			canvasGroup.interactable		= interactable;
			canvasGroup.blocksRaycasts		= blockRaycasts;
			canvasGroup.ignoreParentGroups	= ignoreParentgroups;
		}

		if (forwardInputsOnShow)
			ForwardInput = true;

		onShow?.Invoke();
	}

	protected virtual void OnHide()
	{
		canvas.enabled = false;

		if (canvasGroup)
		{
			canvasGroup.interactable		= false;
			canvasGroup.blocksRaycasts		= false;
			canvasGroup.ignoreParentGroups	= true;
		}

		if (forwardInputsOnShow)
			ForwardInput = false;

		onHide?.Invoke();
	}

	private void SetForwardInput(bool value)
	{
		if (forwardInput == value)
		{
			Debug.LogWarning("Should not get here... :(", this); // Tried to SetForwardInput with the current value
			return;
		}

		if (forwardInput)
			uiController.InputSource.onActionTriggered -= ForwardInput_Internal;
		else
			uiController.InputSource.onActionTriggered += ForwardInput_Internal;

		forwardInput = value;
	}
	private void ForwardInput_Internal(InputAction.CallbackContext callbackContext)
	{
		//Retrieve the action method in the current class instance
#if UNITY_EDITOR
		//Less restriction in editor to be able to detect potential wrong signatures (in which case we emity a warning)
		MethodInfo foundMethod = GetType().GetMethod("On" + callbackContext.action.name, methodFlags);
#else
		MethodInfo foundMethod = GetType().GetMethod("On" + callbackContext.action.name, methodFlags, null, methodParameterTypes, null);
#endif

		if (foundMethod != null)
		{
#if UNITY_EDITOR
			//Check that the function has the right signature
			ParameterInfo[] methodParams = foundMethod.GetParameters();

			if (methodParams.Length != 1 || methodParams[0].ParameterType != typeof(InputAction.CallbackContext))
			{
				Debug.LogWarning($"On{callbackContext.action.name} has the wrong signature, it must take a single parameter of type InputAction.CallbackContext.");
				return;
			}
#endif

			parameters[0] = callbackContext;

			//Call the found method
			foundMethod.Invoke(this, parameters);
		}
	}

	#endregion
}