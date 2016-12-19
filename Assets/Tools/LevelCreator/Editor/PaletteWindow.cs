using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace sweetcli.LevelCreator{
	public class PaletteWindow : EditorWindow{

		public static PaletteWindow instance;
		#region variables for the gui tabs
		List<sweetcli.LevelCreator.Category> categories;
		List<string> categoryLabels;
		Category selectedCategory;
		#endregion

		#region item preview variables
		private string path = "Assets/Prefabs/LevelPieces";
		private string soPath = "Assets/ScriptableObjects";
		//scriptableObjects now!
		List<PaletteItem> items;
		Dictionary<Category,List<PaletteItem>> categorizedItems;
		Dictionary<PaletteItem,Texture2D> itemPreview;
		#endregion

		#region scrollRect and preview button size variables
		Vector2 scrollPosition;
		const float ButtonWidth = 80;
		const float ButtonHeight = 90;
		//TODO add the window size scaler
		#endregion

		#region custom methods

		public static void ShowPaletteWindow(){
			
			//for the EditorWindow object get the type of PaletteWindow,
			// cast to self and we are insuring we only interact with a single
			//paletteWindow.
			instance = (PaletteWindow)EditorWindow.GetWindow (typeof(PaletteWindow));
			//set title and icon using the GUIContent of a Palette.
			instance.titleContent = new GUIContent("Palette");
		}
		/// <summary>
		/// Inits the categories.
		/// </summary>
		private	void InitCategories(){
			//grab all enums from the public enum Category
			categories = RunAndJump.LevelPackager.EditorUtils.GetListFromEnum<Category> ();
			//define and populate
			categoryLabels = new List<string>();
			foreach (Category item in categories) {
				categoryLabels.Add( item.ToString () );
			}

		}
		/// <summary>
		/// Draws the tabs for the palette window gui.
		/// </summary>
		private void DrawTabs(){

			//select the current tab from the available labels
			int index = GUILayout.Toolbar ((int)selectedCategory, categoryLabels.ToArray ());
			//update which is selected
			selectedCategory = categories[index];

		}

		/// <summary>
		/// Inits the content.
		/// </summary>
		private void InitContent(){
			categorizedItems = new Dictionary<Category, List<PaletteItem>> ();
			itemPreview = new Dictionary<PaletteItem, Texture2D> ();
			foreach (var item in categories) {
				categorizedItems.Add (item,
					new List<PaletteItem>()
				);
			}
			items = RunAndJump.LevelPackager.EditorUtils.GetAssetsWithScript<PaletteItem> (path);
			//give items to each category
			foreach (var item in items) {
				categorizedItems [item.category].Add (item);
			}

		}
		private Texture2D[] DisplayPreviewImages(){
			Texture2D[] previewTexture = new Texture2D[categorizedItems [selectedCategory].Count];
			//go throught PaletteItem List at selectedCategory
			int index = 0;
			foreach (var item in categorizedItems [selectedCategory]) {
				previewTexture.SetValue (
					itemPreview [item], index
				);
				index++;
				
			}

			return previewTexture;
		}
		private GUIStyle GetGUIEditorStyle(){
			throw new System.NotImplementedException("TODO");
		}
		/// <summary>
		/// Gets the selected prefab item from categorizedItem based 
		/// on selected Category and selected item by index
		/// </summary>
		/// <param name="index">Index.</param>
		private void GetSelectedPrefabItem(int index = -1){
			if (index >= 0) {
				PaletteItem item = categorizedItems [selectedCategory] [index];
			}
		}

		private void DrawScroll(){
			//no items drop out
			if (categorizedItems [selectedCategory].Count == 0) {
				EditorGUILayout.HelpBox ("This category is empty!", MessageType.Info);
				return;
			}


			//using button size figure out the total counts
			int rowCapacity  = (int)Mathf.Floor (position.width / ButtonWidth);
			int selectedPrefabIndex = -1; // none selected.
			scrollPosition = GUILayout.BeginScrollView (scrollPosition);
			selectedPrefabIndex = GUILayout.SelectionGrid (selectedPrefabIndex, 
				DisplayPreviewImages(),
				rowCapacity,
				GetGUIEditorStyle ()
			);
			//if nothing selected in the selectionGrid, we won't have a prefab to select either
			GetSelectedPrefabItem (selectedPrefabIndex);
			GUILayout.EndScrollView ();

		}

		#endregion
		#region Unity overridable methods.

	///<summary>
	/// Required methods for Editor Gui rendering
	///</summary>
		private void OnEnable(){
			//build list of Categories
			if (categories.Equals (null)) {
				InitCategories ();
			}

			if (categorizedItems.Equals (null)) {
				InitContent ();
			}
		}
		private void OnDisabled(){}
		private void OnDestory(){}

		private void OnGUI(){
			DrawTabs ();
			DrawScroll ();
		}
		private void Update(){}  // I think there is a better, different Update for editor.
		#endregion
	}
}
