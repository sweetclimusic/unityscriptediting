using UnityEditor;

namespace sweetcli.LevelCreator{
public class LevelInspectorUtils : Editor {

//		private Mode selectedMode;
//		private Mode currentMode;
//		public Mode CurrentMode{
//			get {
//				return currentMode;
//			}
//			set{
//				currentMode = value;
//			}
//		}
//		public Mode SelectedMode{
//			get{
//				return selectedMode;
//			}
//			set{
//				selectedMode = value;
//			}
//		}

	}

	public enum Mode{
		Painting,
		View,
		Edit,
		Delete
	}
}