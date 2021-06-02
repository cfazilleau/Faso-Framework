using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneSerializer
{
	#region Variables

	private const string scenePattern	= @".+\/(.+)\.unity";
	private const string assetPath		= "Assets/Settings/SceneInfo.asset";

	#endregion

	#region Methods

	static SceneSerializer()
	{
		//Save scene info right at reload
		SaveScenesInfo();

		//Save when we change build settings 
		EditorBuildSettings.sceneListChanged += SaveScenesInfo;
	}

	private static void SaveScenesInfo()
	{
		//Retrieve the existing asset
		ScenesInfo scenesInfo = AssetDatabase.LoadAssetAtPath<ScenesInfo>(assetPath);

		//If it doesn't exist yet, create it
		if (scenesInfo == null)
		{
			scenesInfo = ScriptableObject.CreateInstance<ScenesInfo>();
			AssetDatabase.CreateAsset(scenesInfo, assetPath);
		}

		//Get build settings
		string[] sceneNames = EditorBuildSettings.scenes
									.Where(scene => scene.enabled)
									.Select(scene => Regex.Match(scene.path, scenePattern).Groups[1].Value)
									.ToArray();

		//Write if different
		if (scenesInfo.sceneNames == null || !sceneNames.SequenceEqual(scenesInfo.sceneNames))
		{
			scenesInfo.sceneNames = sceneNames;
			EditorUtility.SetDirty(scenesInfo);
		}
	}

	#endregion
}
