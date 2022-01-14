using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace FasoFramework
{
	[DefaultExecutionOrder(-97)]
	public class GameflowManager : MonoBehaviour
	{
		#region Variables

		[SerializeField]
		private PlayerController playerController = null;

		[SerializeField]
		private CameraController cameraController = null;

		[SerializeField]
		private UIController uiController = null;

		[SerializeField]
		private Manager[] managers = null;

		#endregion

		#region Properties

		public PlayerController PlayerController => playerController;
		public CameraController CameraController => cameraController;
		public UIController UIController => uiController;

		#endregion

		#region Unity Methods

		protected virtual void Awake()
		{
			//Update the current gameflow manager for this scene
			GameInstance.GameflowManager = this;
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
		*	Return the current PlayerController cast to the provided type if valid.
		*	If there is no player controller or if the provided type doesn't match with the current PlayerController type, null is returned.
		*/
		public T GetPlayerController<T>() where T : PlayerController
		{
			return playerController as T;
		}

		/**
		*	Return the current CameraController cast to the provided type if valid.
		*	If there is no camera controller or if the provided type doesn't match with the current CameraController type, null is returned.
		*/
		public T GetCameraController<T>() where T : CameraController
		{
			return cameraController as T;
		}

		/**
		*	Return the current UIController cast to the provided type if valid.
		*	If there is no UI manager or if the provided type doesn't match with the current UIController type, null is returned.
		*/
		public T GetUIController<T>() where T : UIController
		{
			return uiController as T;
		}

		/**
		*	Return the queried manager if it is contained in this GameflowManager, else null.
		*/
		public T GetManager<T>(bool considerSubclass = true) where T : Manager
		{
			return GetManager(typeof(T), considerSubclass) as T;
		}

		public Manager GetManager(Type managerType, bool considerSubclass = true)
		{
			//Make sure the queried type is actually a subclass of Manager
#if UNITY_EDITOR
			if (!managerType.IsSubclassOf(typeof(Manager)))
				Debug.LogWarning($"Used GameflowManager.GetManager(type) with {managerType.Name} which doesn't inherit from Manager.");
#endif

			foreach (Manager manager in managers)
				if (manager.GetType() == managerType || (considerSubclass && manager.GetType().IsSubclassOf(managerType)))
					return manager;

			return null;
		}

#if UNITY_EDITOR
		[NaughtyAttributes.Button]
		protected void RefreshFrameworkReferences()
		{
			//Try to find and assign missing references
			//Player Controller
			if (playerController == null)
				playerController = GetComponentInScene<PlayerController>();

			//Camera Controller
			if (cameraController == null)
				cameraController = GetComponentInScene<CameraController>();

			//UI Manager
			if (uiController == null)
				uiController = GetComponentInScene<UIController>();
		}

		[NaughtyAttributes.Button]
		protected void RefreshManagerReferences()
		{
			//Managers
			managers = GetComponentsInScene<Manager>().ToArray();
		}

		private T GetComponentInScene<T>() where T : MonoBehaviour
		{
			Scene editorScene = EditorSceneManager.GetActiveScene();
			T result = null;

			GameObject[] rootGameObjects = editorScene.GetRootGameObjects();

			foreach (GameObject go in rootGameObjects)
			{
				result = go.GetComponentInChildren<T>(true);

				if (result != null)
				{
					break;
				}
			}

			return result;
		}

		private List<T> GetComponentsInScene<T>() where T : MonoBehaviour
		{
			Scene editorScene = EditorSceneManager.GetActiveScene();
			List<T> result = new List<T>();

			GameObject[] rootGameObjects = editorScene.GetRootGameObjects();

			foreach (GameObject go in rootGameObjects)
			{
				result.AddRange(go.GetComponentsInChildren<T>(true));
			}

			return result;
		}

#endif

		#endregion
	}
}