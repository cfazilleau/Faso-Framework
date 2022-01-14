using System;

using UnityEngine;


namespace FasoFramework
{
	public class ScenesInfo : ScriptableObject
	{
		#region Variables

		[NaughtyAttributes.ReadOnly]
		public string[] sceneNames = null;

		#endregion

		#region Methods

		public ScenesInfo(string[] sceneNames)
		{
			this.sceneNames = sceneNames;
		}

		public int GetScene(string sceneName)
		{
			return Array.IndexOf(sceneNames, sceneName);
		}

		public string GetSceneName(int sceneBuildIndex)
		{
			Debug.Assert(sceneBuildIndex >= 0 && sceneBuildIndex < sceneNames.Length);

			return sceneNames[sceneBuildIndex];
		}

		#endregion
	}
}