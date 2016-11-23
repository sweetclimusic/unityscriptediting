using UnityEngine;
using UnityEditor;
using System.Collections;


namespace sweetcli.LevelCreator {
	/// <summary>
	/// Menu items. store and define all entry points to editor scripts
	/// </summary>
	public static class MenuItems {
		//create a new scene
		[MenuItem ("Tools/LevelCreator/New Level")]
		public static void CreateLevel(){
			LevelCreatorUtils.NewLevel();
		}
		public static void ResetLevel(){
			LevelCreatorUtils.ResetLevel();
		}

	}
}
