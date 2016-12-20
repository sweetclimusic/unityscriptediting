using UnityEngine;

namespace sweetcli.LevelCreator {
	/// <summary>
	/// Palette item. created as a MonoBehaviour.
	/// </summary>
	public class PaletteItem : MonoBehaviour {
		#if UNITY_EDITOR
		public string itemName;
		//default to misc
		public Category category = Category.Misc;
		public Object inspectedScript;

		public string ItemName{ 
			get{return itemName;}
			set{ itemName = value;}
		}

		public Object InspectedScript{ 
			get{return inspectedScript;}
			set{ inspectedScript = value;}
		}

		public void Awake(){
			if (InspectedScript.Equals (null) && GetComponent < MonoBehaviour > () != null) {
				foreach (var item in GetComponents<MonoBehaviour> () ) {
					//item may be a script...
					if (item.name.Contains ("Controller")) {
						InspectedScript = item;
						break;
					}
				}

			}
		}
		#endif
	}

	[System.Serializable]
	public enum Category{
		Misc,
		Collectables,
		Enemies,
		Blocks
	}
}