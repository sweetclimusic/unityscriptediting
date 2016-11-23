using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using RunAndJump;

namespace sweetcli.LevelCreator {
	public static class LevelCreatorUtils {
		

		 static void CreateScene(){
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();

			//create a new empty scene
			EditorSceneManager.NewScene ( 
				NewSceneSetup.DefaultGameObjects,NewSceneMode.Single
			);
			//same time we create the scene add the first component.
			GameObject levelScript = new GameObject("Level");
			levelScript.transform.position = Vector3.zero;
			levelScript.AddComponent <RunAndJump.Level>();
		}

		//Obsolete maybe due to using EditorSceneManager instead of how the example does it.
		//so called from a new menu item now.
		 static void ClearScene(){
			GameObject[] objects = Object.FindObjectsOfType<GameObject> ();
			//remove all gameobjects we found
			foreach (var item in objects) {
				//Like Destroy but for the Editor. NICE!
				GameObject.DestroyImmediate (item);
			}
		}


		public static void NewLevel(){
			CreateScene ();
			//ClearScene ();
		}
		public static void ResetLevel(){
			ClearScene ();
		}



	}
}