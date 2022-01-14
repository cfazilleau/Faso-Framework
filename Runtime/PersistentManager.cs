using UnityEngine;
using UnityEngine.SceneManagement;


namespace FasoFramework
{
	[DefaultExecutionOrder(-98)]
	public class PersistentManager : MonoBehaviour
	{
		#region Variables

		#endregion

		#region Properties

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
		*	Called by the game instance once a new scene has been swapped in.
		*/
		public virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			//Default implementation does nothing
		}

		/**
		*	Called by the game instance just before loading a new scene.
		*/
		public virtual void OnSceneExit()
		{
			//Default implementation does nothing
		}

		#endregion
	}
}