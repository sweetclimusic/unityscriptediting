using UnityEngine;
using UnityEditor;


namespace sweetcli.LevelCreator {
	/// <summary>
	/// Menu items. store and define all entry points to editor scripts
	/// </summary>
	public static class MenuItems {
		//create a new scene
		[MenuItem ("Tools/Level Creator/New Level %_l")]
		public static void CreateLevel(){
			LevelCreatorUtils.NewLevel();
		}
		[MenuItem ("Tools/Level Creator/Show Palette _p")]
		public static void ShowPaletteWindow(){
			PaletteWindow.ShowPaletteWindow();
		}

		[MenuItem ("Tools/Level Creator/Reset Level %_#_r")]
		public static void ResetLevel(){
			LevelCreatorUtils.ResetLevel();
		}
		[MenuItem ("Tools/Level Creator/New Level Settings")]
		private static void NewLevelSettings () {
			string path = EditorUtility.SaveFilePanelInProject(
				"New Level Settings",
				"LevelSettings",
				"asset",
				"Define the name for the LevelSettings asset");
			if(path != "") {
				RunAndJump.LevelPackager.EditorUtils.CreateAsset<RunAndJump.LevelSettings>(path);
			}
		}
//      adding shortcuts
//		String Key
//		%  -- Ctrl on Windows / Command on OSX
//		#  -- Shift
//		&  -- Alt
//		LEFT/RIGHT/UP/DOWN -- Arrow keys
//		F1…F2 -- F keys
//		HOME, END, PGUP, PGDN -- Home, End, Page Up, Page Down
	}
}
