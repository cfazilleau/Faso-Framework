using System;
using System.Reflection;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.InputSystem;


namespace FasoFramework
{
	[DefaultExecutionOrder(-90)]
	public abstract class Controller : MonoBehaviour
	{
		#region Variables

		[SerializeField, OnValueChanged("OnInputSourceChanged"), Tooltip("PlayerInput this controller should react to.")]
		private PlayerInput startInputSource = null;

		private PlayerInput inputSource = null;

		private BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private Type[] methodParameterTypes = new Type[] { typeof(InputAction.CallbackContext) };
		private object[] parameters = new object[1];

		public static event Action<InputActionMap> onChangedActionMap = null;

		#endregion

		#region Properties

		[ShowNativeProperty]
		public PlayerInput InputSource
		{
			get => inputSource;
			set
			{
				//Unregister from previous input source
				if (inputSource != null)
					UnsubscribeFromPlayerInput(inputSource);

				inputSource = value;

				//Listen to inputs coming from the new input source
				if (inputSource != null)
					SubscribeToPlayerInput(inputSource);
			}
		}

		#endregion

		#region Unity Methods

		protected virtual void Awake()
		{
			if (startInputSource != null && inputSource == null)
			{
				InputSource = startInputSource;
			}
			//else
			//	Debug.LogWarningFormat("You must specify an InputSource in {0}.Controller to catch inputs.", gameObject.name);
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

		public void EnableInputs()
		{
			InputSource?.ActivateInput();
		}

		public void DisableInputs()
		{
			InputSource?.DeactivateInput();
		}

		public void SetActionMap(string inputMapName)
		{
			if (InputSource != null)
			{
				//Debug.Log($"Set input map to {inputMapName}");
				InputSource.SwitchCurrentActionMap(inputMapName);
				onChangedActionMap?.Invoke(InputSource.currentActionMap);
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning($"SetActionMap map to {inputMapName} failed because InputSource is null.");
			}
#endif
		}

		public void SetActionMap(InputActionMap inputActionMap)
		{
			SetActionMap(inputActionMap.name);
		}

		protected virtual void OnControlsChanged(PlayerInput playerInput)
		{
			//Default implementation does nothing for now
		}

		protected virtual void OnDeviceLost(PlayerInput playerInput)
		{
			//Default implementation does nothing for now
		}

		protected virtual void OnDeviceRegained(PlayerInput playerInput)
		{
			//Default implementation does nothing for now
		}

		private void SubscribeToPlayerInput(PlayerInput playerInput)
		{
			playerInput.onActionTriggered += ForwardInput;
			playerInput.onControlsChanged += OnControlsChanged;
			playerInput.onDeviceLost += OnDeviceLost;
			playerInput.onDeviceRegained += OnDeviceRegained;
		}

		private void UnsubscribeFromPlayerInput(PlayerInput playerInput)
		{
			playerInput.onActionTriggered -= ForwardInput;
			playerInput.onControlsChanged -= OnControlsChanged;
			playerInput.onDeviceLost -= OnDeviceLost;
			playerInput.onDeviceRegained -= OnDeviceRegained;
		}

		private void ForwardInput(InputAction.CallbackContext callbackContext)
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

		protected void OnInputSourceChanged()
		{
			//Setup the input source
			if (inputSource != null)
				inputSource.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
		}

		#endregion
	}
}