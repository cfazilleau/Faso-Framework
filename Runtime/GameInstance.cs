using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[DefaultExecutionOrder(-100)]
public class GameInstance : MonoBehaviour
{
	#region Variables

	[SerializeField, Tooltip("Asset containing all scenes name/index.")]
	private ScenesInfo			scenesInfo	= null;

	[SerializeField]
	private PersistentManager[]		managers			= null;

	private static GameInstance		instance			= null;
	
	private GameflowManager			gameflowMgr			= null;

	private Coroutine				sceneLoading		= null;
	private string					toLoadSceneName		= string.Empty;

	private event Action<float>		onSceneLoading		= null;

	#endregion

	#region Properties

	public static GameflowManager GameflowManager
	{
		get => (instance != null) ? instance.gameflowMgr : null;
		set => instance.gameflowMgr = value;
	}

	public static ScenesInfo ScenesInfo => instance.scenesInfo;

	public static GameObject GameObject => (instance != null) ? instance.gameObject : null;

	public static event Action<float> OnSceneLoading
	{
		add => instance.onSceneLoading += value;
		remove => instance.onSceneLoading -= value;
	}

	private bool IsLoadingScene => toLoadSceneName.Length > 0;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		if (instance == null)
			Initialize();
		else
			//A game manager already exists, delete this one
			Destroy(gameObject);
	}

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	#endregion

	#region Custom Methods

	/**
	*	Return the current GameflowManager cast to the provided type if valid.
	*	If there is no gameflow manager or if the provided type doesn't match with the current GameflowManager type, null is returned.
	*/
	public static T GetGameflowManager<T>() where T : GameflowManager
	{
		return (instance != null) ? instance.gameflowMgr as T : null;
	}

	/**
	*	Return the current PlayerController cast to the provided type if valid.
	*	If there is no gameflow manager, no player controller, or if the provided type doesn't match with the current PlayerController type, null is returned.
	*/
	public static T GetPlayerController<T>() where T : PlayerController
	{
		return (instance != null && instance.gameflowMgr != null) ? instance.gameflowMgr.GetPlayerController<T>() : null;
	}

	/**
	*	Return the current CameraController cast to the provided type if valid.
	*	If there is no gameflow manager, no camera controller, or if the provided type doesn't match with the current CameraController type, null is returned.
	*/
	public static T GetCameraController<T>() where T : CameraController
	{
		return (instance != null && instance.gameflowMgr != null) ? instance.gameflowMgr.GetCameraController<T>() : null;
	}

	/**
	*	Return the current Character cast to the provided type if valid.
	*	If there is no gameflow manager, no player controller, no character, or if the provided type doesn't match with the current Character type, null is returned.
	*/
	public static T GetCharacter<T>() where T : Character
	{
		return (instance != null && instance.gameflowMgr != null && instance.gameflowMgr.PlayerController != null) ? instance.gameflowMgr.PlayerController.GetControlledCharacter<T>() : null;
	}

	/**
	*	Return the current UIController cast to the provided type if valid.
	*	If there is no gameflow manager, no UI manager, or if the provided type doesn't match with the current UIController type, null is returned.
	*/
	public static T GetUIController<T>() where T : UIController
	{
		return (instance != null && instance.gameflowMgr != null) ? instance.gameflowMgr.GetUIController<T>() : null;
	}

	/**
	*	Return the queried persistent manager if it is contained in this GameInstance, else null.
	*/
	public static T GetPersistentManager<T>(bool considerSubclass = true) where T : PersistentManager
	{
		return (instance != null) ? GetPersistentManager(typeof(T), considerSubclass) as T : null;
	}

	/**
	*	Return the queried manager if it is contained in this GameInstance, else null.
	*/
	public static T GetManager<T>(bool considerSubclass = true) where T : Manager
	{
		return (instance != null && instance.gameflowMgr != null) ? instance.gameflowMgr.GetManager<T>() : null;
	}

	public static PersistentManager GetPersistentManager(Type managerType, bool considerSubclass = true)
	{
		if (instance != null)
		{
			//Make sure the queried type is actually a subclass of PersistentManager
#if UNITY_EDITOR
			if (!managerType.IsSubclassOf(typeof(PersistentManager)))
				Debug.LogWarning($"Called GameInstance.GetPersistentManager(type) with {managerType.Name} which doesn't inherit from PersistentManager.");
#endif

			foreach (PersistentManager manager in instance.managers)
				if (manager.GetType() == managerType || (considerSubclass && manager.GetType().IsSubclassOf(managerType)))
					return manager;
		}

		return null;
	}

	public static void LoadScene(string sceneName)
	{
		Debug.Assert(instance != null);

		Debug.Log($"Loading scene \"{sceneName}\"");
		instance.LoadSceneInternal(sceneName);
	}

	public static void LoadScene(int sceneBuildIndex)
	{
		Debug.Assert(instance != null);

		LoadScene(instance.scenesInfo.GetSceneName(sceneBuildIndex));
	}

	public static void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private void Initialize()
	{
		instance = this;

		//Create a copy of this asset just in case.
		Debug.Assert(scenesInfo != null);
		scenesInfo = Instantiate(scenesInfo);

		//Receive a callback when a new scene is loaded
		SceneManager.sceneLoaded += OnSceneLoaded;

		transform.SetParent(null);
		DontDestroyOnLoad(gameObject);
	}

	private void LoadSceneInternal(string sceneName)
	{
		if (!IsLoadingScene)
		{
			toLoadSceneName = sceneName;

			OnLoadingScreenDisplayed();
		}
	}

	private void OnLoadingScreenDisplayed()
	{
		sceneLoading = StartCoroutine(SceneLoadingRoutine(toLoadSceneName));
	}

	private void OnNewSceneReady()
	{
		sceneLoading = null;
		toLoadSceneName = string.Empty;
	}

	//Called just after we entered a scene (after all scene scripts have called Awake)
	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		//Callback on all persistent managers to inform a new scene has been loaded
		foreach (PersistentManager manager in managers)
			manager.OnSceneLoaded(scene, loadSceneMode);
	}

	//Called just before we exit a scene
	private void OnSceneExit()
	{
		//Callback on all persistent managers to inform we are exiting the current scene
		foreach (PersistentManager manager in managers)
			manager.OnSceneExit();
	}

	private IEnumerator SceneLoadingRoutine(string sceneName)
	{
		OnSceneExit();

		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

		while (!async.isDone)
		{
			onSceneLoading?.Invoke(async.progress);
			yield return null;
		}

		//Wait for next frame to avoid scene swap freeze
		yield return null;

		// Force loading progress to 100%
		onSceneLoading?.Invoke(1f);
		OnNewSceneReady();
	}

	#endregion

	#region Editor

#if UNITY_EDITOR
	[NaughtyAttributes.Button]
	protected void RefreshManagerReferences()
	{
		//Managers
		managers = GetComponentsInScene<PersistentManager>().ToArray();
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
