using UnityEngine;
using UnityEditor;
//using System.Linq;
using System.Collections.Generic;

namespace RunAndJump.LevelPackager {
	public static class EditorUtils {

		public static void Commit (LevelsPackage package, System.Action successCb = null, System.Action errorCb = null) {
			try {
				List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene> (EditorBuildSettings.scenes);
				List<EditorBuildSettingsScene> levelScenes = new List<EditorBuildSettingsScene> ();
				List<EditorBuildSettingsScene> buildScenesToRemove = new List<EditorBuildSettingsScene> ();
				List<LevelMetadata> levelListmetadatasToRemove = new List<LevelMetadata> ();
				
				// Check level scenes in original Build Scenes
				foreach (EditorBuildSettingsScene scene in buildScenes) {
					if (scene.path.Contains (LevelsPackage.Suffix)) {
						buildScenesToRemove.Add (scene);
					}
				}
				// Remove level scenes
				foreach (EditorBuildSettingsScene scene in buildScenesToRemove) {
					buildScenes.Remove (scene);
				}
				// Create Scenes references based on LevelMatadata, and in the process keep track of the ones with null value
				foreach (LevelMetadata metadata in package.metadataList) {
					if (metadata.scene != null) {
						string pathToScene = AssetDatabase.GetAssetPath (metadata.scene);
						EditorBuildSettingsScene scene = new EditorBuildSettingsScene (pathToScene, true);
						levelScenes.Add (scene);
					} else {
						levelListmetadatasToRemove.Add (metadata);
					}
				}
				// Removing null value metadatas from levelList
				foreach (LevelMetadata metadata in levelListmetadatasToRemove) {
					package.metadataList.Remove (metadata);
				}
				// Removing duplicated from levelScenes
				// levelScenes = levelScenes.Distinct().ToList<EditorBuildSettingsScene>();
				// Comminting changes
				buildScenes.AddRange (levelScenes);
				EditorBuildSettings.scenes = buildScenes.ToArray ();
				if (successCb != null) {
					successCb ();
				}
			} catch (UnityException) {
				if (errorCb != null) {
					errorCb ();
				}
			}
		}

		///<summary>
		/// Uses a list of generics as available scripts object select themselves
		///Find Prefab in given path. 
		///</summary>
		public static List<T> GetAssetsWithScript<T> (string path) where T : MonoBehaviour {
			T tmpObjectType;
			string assetPath;
			GameObject assetPrefab;
			List<T> listOfAssests = new List<T> ();

			//for filters, searching for a `types`, the keyword, or key character is (t).
			// so we search the AssetDatabase with a filter on all prefab types in the path location.
			string[] guids = AssetDatabase.FindAssets ("t:Prefab", new string[] {path});
			foreach (var prefabID in guids) {
				//get the path of the prefab found
				assetPath = AssetDatabase.AssetPathToGUID (prefabID);
				//get the actual Gameobject of the asset at path location
				assetPrefab = AssetDatabase.LoadAssetAtPath (assetPath,typeof(GameObject))as GameObject;
				tmpObjectType = assetPrefab.GetComponent<T> ();
				if(tmpObjectType != null){
					listOfAssests.Add (tmpObjectType);
				}
			}
			return listOfAssests;
		}
		/// <summary>
		/// Gets the list from enum.
		/// </summary>
		/// <returns>The list from enum.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> GetListFromEnum<T>(){
			List<T> enumList = new List<T>();
			//don't get this part..
			//other that a system array called enums gets all enum values of the T type
			//but don't get how it works, a compiler thing that populates System.Enum?
			// so on runtime System.Enum.GetValues and T == Misc Enum. all enums of the type is found with getValues?
			//Checking MSDN confirms this theory as a enumType is a constant and is stored in an array.
			System.Array enums = System.Enum.GetValues (typeof(T));
			foreach (T item in enums) {
				enumList.Add (item);
			}
			return enumList;
		}
	}
}