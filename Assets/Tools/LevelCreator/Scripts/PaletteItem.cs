using UnityEngine;

namespace sweetcli.LevelCreator {
	/// <summary>
	/// Palette item. created as a ScriptableObject incase we need to expand the funtionality of prefabs later on.
	/// </summary>
	[CreateAssetMenu(menuName = "Palette Item")]
	public class PaletteItem : ScriptableObject {
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