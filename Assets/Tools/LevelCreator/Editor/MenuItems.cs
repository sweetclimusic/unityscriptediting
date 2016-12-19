using UnityEngine;
using UnityEditor;


namespace sweetcli.LevelCreator {
	/// <summary>
	/// Menu items. store and define all entry points to editor scripts
	/// </summary>
	public static class MenuItems {
		//create a new scene
		[MenuItem ("Tools/LevelCreator/New Level %_l")]
		public static void CreateLevel(){
			LevelCreatorUtils.NewLevel();
		}
		[MenuItem ("Tools/LevelCreator/Show Palette _p")]
		public static void ShowPaletteWindow(){
			PaletteWindow.ShowPaletteWindow();
		}

		[MenuItem ("Tools/LevelCreator/Reset Level %_#_r")]
		public static void ResetLevel(){
			LevelCreatorUtils.ResetLevel();
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
