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

		#region event delegates
		//how to do this as a closure/lambda expression again?
		public delegate void paletteItemSelectedDelegate(PaletteItem item,Texture2D preview);
		public static event paletteItemSelectedDelegate PaletteItemSelectedEvent;

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

		/// <summary>
		/// Generates the prefab previews. using AssetPreview.
		/// any prefab with the PaletteItem script, get it's preview texture.
		/// </summary>
		private void GeneratePrefabPreviews (){
			//finally fill itemPreview
			foreach (PaletteItem item in items) {
				//verify the current paletteItem exists as a key
				if (!itemPreview.ContainsKey (item)) {
					itemPreview.Add (
						item,
						AssetPreview.GetAssetPreview (
							item.gameObject
					) as Texture2D);
				}
			}
		}

		/// <summary>
		/// Displaies the content of the GUI.
		/// </summary>
		/// <returns>The GUI content.</returns>
		private GUIContent[] DisplayGUIContent(){
			//generate a list

			List<GUIContent> previewTexture = new List<GUIContent> ();
			if (itemPreview.Count == items.Count) {
				
			
				int totalItems = categorizedItems [selectedCategory].Count;
				//go throught PaletteItem List at selectedCategory
				for (int index = 0; index < totalItems; index++) {
					//add GUI Content that inclused the itemName and image preview
					//when a current category is selected and the current item index
					previewTexture.Add (
						new GUIContent (
							categorizedItems [selectedCategory] [index].ItemName,
							itemPreview [categorizedItems [selectedCategory] [index]]
						)

					);

				}
			}

			return previewTexture.ToArray ();
		}
		/// <summary>
		/// Gets the GUI editor style.
		/// styles a default button to place the image over the label ,centrally align the text of the label.
		/// </summary>
		/// <returns>The GUI editor style.</returns>
		private GUIStyle GetGUIEditorStyle(){
			GUIStyle guiStyle = new GUIStyle (GUI.skin.button);
			guiStyle.alignment = TextAnchor.LowerCenter;
			guiStyle.imagePosition = ImagePosition.ImageAbove;
			guiStyle.fixedWidth = ButtonWidth;
			guiStyle.fixedHeight = ButtonHeight;
			return guiStyle;
		}
		/// <summary>
		/// Gets the selected prefab item from categorizedItem based 
		/// on selected Category and selected item by index
		/// </summary>
		/// <param name="index">Index.</param>
		private void GetSelectedPrefabItem(int index = -1){
			if (index != -1) {
				PaletteItem item = categorizedItems [selectedCategory] [index];
				//monitor the event
				if(PaletteItemSelectedEvent != null){
					PaletteItemSelectedEvent(item,itemPreview[item]);
				}
			}

		}
		/// <summary>
		/// Draws the scroll.
		/// </summary>
		private void DrawScroll(){
			//no items drop out
			if (categorizedItems [selectedCategory].Count == 0) {
				EditorGUILayout.HelpBox ("This category is empty!", MessageType.Info);
				return;
			}


			//using button size figure out the total counts
			int rowCapacity  = Mathf.FloorToInt (position.width / ButtonWidth);
			int selectedPrefabIndex = -1; // none selected.
			//render and get current position
			scrollPosition = GUILayout.BeginScrollView (scrollPosition);
			//render selection grid and get current selected item
			selectedPrefabIndex = GUILayout.SelectionGrid (selectedPrefabIndex, 
				DisplayGUIContent(),
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
			if (categories == null) {
				InitCategories ();
			}

			if (categorizedItems == null) {
				InitContent ();
			}
		}
		private void OnDisabled(){}
		private void OnDestory(){}

		private void OnGUI(){
			DrawTabs ();
			DrawScroll ();
		}
		private void Update(){
			if (itemPreview.Count != items.Count) {
				GeneratePrefabPreviews ();
			}
		}  // I think there is a better, different Update for editor.
		#endregion
	}
}
