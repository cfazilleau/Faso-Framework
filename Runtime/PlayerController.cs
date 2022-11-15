using NaughtyAttributes;
using System;
using UnityEngine;


namespace FasoFramework
{
	public abstract class PlayerController : Controller
	{
		#region Variables

		[SerializeField, Tooltip("Character controlled by this PlayerController when the game starts. Can be null.")]
		private Character startControlledCharacter = null;

		private Character controlledCharacter = null;

		#endregion

		#region Properties

		[ShowNativeProperty]
		public Character ControlledCharacter
		{
			get
			{
				#if UNITY_EDITOR
				if (controlledCharacter == null)
					return startControlledCharacter;
				#endif
				return controlledCharacter;
			}
				
				
			set => Possess(value);
		}

		#endregion

		#region Unity Methods

		protected override void Start()
		{
			base.Start();

			if (startControlledCharacter != null)
				Possess(startControlledCharacter);
		}

		#endregion

		#region Custom Methods

		/**
		*	Takes the control of a character.
		*	If a character is already controlled when calling this method, it is unpossessed first.
		*/
		public void Possess(Character character)
		{
			Debug.Assert(character != null, "PlayerController.Possess can't be called with a null character. To unpossess the controlled character, use PlayerController.Unpossess instead.");

			if (controlledCharacter != null)
				Unpossess();

			controlledCharacter = character;

			controlledCharacter.OnPossessed(this);
		}

		/**
		*	Unpossess the currently controlled character.
		*/
		public void Unpossess()
		{
			if (controlledCharacter != null)
				controlledCharacter.OnUnpossessed(this);

			controlledCharacter = null;
		}

		/**
		*	Return the controlled Character cast to the provided type if valid.
		*	If the provided type doesn't match with the current Character type, null is returned.
		*/
		public T GetControlledCharacter<T>() where T : Character
		{
			return ControlledCharacter as T;
		}

		#endregion
	}
}