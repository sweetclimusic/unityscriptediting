using UnityEngine;
using UnityEditor;


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
		[MenuItem ("Tools/LevelCreator/Reset Level")]
		public static void ResetLevel(){
			LevelCreatorUtils.ResetLevel();
		}

	}
}
