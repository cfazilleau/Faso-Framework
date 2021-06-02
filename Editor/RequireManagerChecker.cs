using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using System.Reflection;

[InitializeOnLoad]
public static class RequireManagerChecker
{
	#region Methods

	// register an event handler when the class is initialized
	static RequireManagerChecker()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		//Check scripts of the loaded scene
		CheckRequireManagerAttributeInScene(scene);

		if (GameInstance.GameObject == null)
		{
			Debug.LogWarning("GameInstance is missing in this scene.");
			return;
		}

		//Check scripts of the DontDestroyOnLoad scene
		CheckRequireManagerAttributeInScene(GameInstance.GameObject.scene);
	}

	/**
	*	Check RequireManager as well as RequirePersistentManager attributes
	*/
	private static void CheckRequireManagerAttributeInScene(Scene scene)
	{
		HashSet<Type> checkedTypes = new HashSet<Type>();
		Type currentType;
		MonoScript script;

		//Iterate over all monobehaviours to find RequireManager and RequirePersistentManager attributes
		foreach (GameObject go in scene.GetRootGameObjects())
		{
			foreach (Component component in go.GetComponentsInChildren<Component>(true))
			{
				if (component == null)
				{
					Debug.LogError($"Missing script in children of gameobject {go.name}", go);
					continue;
				}

				if (component is MonoBehaviour monoBehaviour)
				{
					// Check RequireLayer Attribute on every instance
					Attribute attr = monoBehaviour.GetType().GetCustomAttribute(typeof(RequireLayerAttribute), true);
					if (attr != null && monoBehaviour.gameObject.layer != (attr as RequireLayerAttribute).LayerMask)
					{
						if ((attr as RequireLayerAttribute).LayerMask == -1)
							Debug.LogError($"Layer Name does not exist: {(attr as RequireLayerAttribute).LayerName}", go);
							
						Debug.LogError($"wrong layer on object {monoBehaviour.gameObject.name}, needs to be set to {LayerMask.LayerToName((attr as RequireLayerAttribute).LayerMask)}", monoBehaviour.gameObject);
					}

					currentType = monoBehaviour.GetType();

					if (!checkedTypes.Contains(currentType))
					{
						checkedTypes.Add(currentType);

						script = MonoScript.FromMonoBehaviour(monoBehaviour);

						//Check that the script manager dependencies are satisfied
						//If something wrong happens (GameInstance or GameflowManager missing), abort the check
						if (!CheckRequirePersistentManagerAttribute(script) || !CheckRequireManagerAttribute(script))
							return;
					}
				}
			}
		}
	}

	private static bool CheckRequireManagerAttribute(MonoScript script)
	{
		object[] attributes = script.GetClass().GetCustomAttributes(typeof(RequireManagerAttribute), true);

		if (attributes.Length > 0)
		{
			//Issue warning if the GameInstance or GameflowMgr is missing
			if (GameInstance.GameflowManager == null)
			{
				Debug.LogWarning("GameInstance or GameflowMgr is missing in this scene.");
				return false;
			}

			foreach (RequireManagerAttribute attribute in attributes)
			{
				CheckRequireManagerAttribute(script.GetClass(), attribute.requiredManagerType1);
				CheckRequireManagerAttribute(script.GetClass(), attribute.requiredManagerType2);
				CheckRequireManagerAttribute(script.GetClass(), attribute.requiredManagerType3);
				CheckRequireManagerAttribute(script.GetClass(), attribute.requiredManagerType4);
				CheckRequireManagerAttribute(script.GetClass(), attribute.requiredManagerType5);
			}
		}

		return true;
	}

	private static void CheckRequireManagerAttribute(Type monobehaviourType, Type requiredManagerType)
	{
		//Abort if the required type is not a subclass of Manager
		if (requiredManagerType != null)
		{
			if (!requiredManagerType.IsSubclassOf(typeof(Manager)))
			{
				Debug.LogWarning($"Used RequireManager with type {requiredManagerType.Name} in {monobehaviourType.Name} but {requiredManagerType.Name} is not a subclass of Manager.");
				return;
			}	

			if (GameInstance.GameflowManager.GetManager(requiredManagerType, true) == null)
				Debug.LogWarning($"{monobehaviourType.Name} requires {requiredManagerType.Name} but its reference is missing in the GameflowManager.");
		}
	}

	private static bool CheckRequirePersistentManagerAttribute(MonoScript script)
	{
		object[] attributes = script.GetClass().GetCustomAttributes(typeof(RequirePersistentManagerAttribute), true);

		if (attributes.Length > 0)
		{
			//Issue warning if the GameInstance is missing
			if (GameInstance.GameObject == null)
			{
				Debug.LogWarning("GameInstance is missing in this scene.");
				return false;
			}

			foreach (RequirePersistentManagerAttribute attribute in attributes)
			{
				CheckRequirePersistentManagerAttribute(script.GetClass(), attribute.requiredManagerType1);
				CheckRequirePersistentManagerAttribute(script.GetClass(), attribute.requiredManagerType2);
				CheckRequirePersistentManagerAttribute(script.GetClass(), attribute.requiredManagerType3);
				CheckRequirePersistentManagerAttribute(script.GetClass(), attribute.requiredManagerType4);
				CheckRequirePersistentManagerAttribute(script.GetClass(), attribute.requiredManagerType5);
			}
		}
		
		return true;
	}

	private static void CheckRequirePersistentManagerAttribute(Type monobehaviourType, Type requiredManagerType)
	{
		//Abort if the required type is not a subclass of Manager
		if (requiredManagerType != null)
		{
			if (!requiredManagerType.IsSubclassOf(typeof(PersistentManager)))
			{
				Debug.LogWarning($"Used RequirePersistentManager with type {requiredManagerType.Name} in {monobehaviourType.Name} but {requiredManagerType.Name} is not a subclass of PersistentManager.");
				return;
			}

			if (GameInstance.GetPersistentManager(requiredManagerType, true) == null)
				Debug.LogWarning($"{monobehaviourType.Name} requires {requiredManagerType.Name} but its reference is missing in the GameInstance.");
		}
	}

	#endregion
}