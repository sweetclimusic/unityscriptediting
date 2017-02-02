using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using RunAndJump;

namespace sweetcli.LevelCreator {
	//overrite the inspector for RunAndJump.Level when ever there is a level.
	[CustomEditor(typeof(RunAndJump.Level))]
	public class LevelInspector : Editor{
		//instance of the current level
		private Level targetLevel;
		#region consistant data
		private int[][] renderedLevelGrid;
		[SerializeField]
		private List<LevelPiece> prefabList;
		#endregion
		private LevelInspectorSceneGui sceneGuiTool;
		//storage
		private int oldColumnSize;
		private int oldRowSize;
		private int minValue;
		private SerializedObject s_Object;
		private SerializedProperty s_TotalTimeProperty;
		
		#region prefab selection event
		private PaletteItem paletteItemSelected;
		private PaletteItem paletteItemInspected;
		private Texture2D itemPreview;
		#endregion
		#region delegates
		private Dictionary<Mode,System.Action<int,int>> ModeActions;
		#endregion
		#region gui handle tool
		private int	originalPosX;
		private int	originalPosY;
		#endregion
		#region property
		public int PrefColumnSize{
			get{
				//check the keyExist.
				if (EditorPrefs.HasKey ("sweetcli_columnSize")) {
					return EditorPrefs.GetInt ("sweetcli_columnSize"); 
				} else {
					return oldColumnSize;
				}
			}
			set{
				oldColumnSize = value;
				EditorPrefs.SetInt ("sweetcli_columnSize", (int)Mathf.Max (0, value) );
			}
		}
		public int PrefRowSize{
			get{
				//check the keyExist.
				if (EditorPrefs.HasKey ("sweetcli_rowSize")) {
					return EditorPrefs.GetInt ("sweetcli_rowSize"); 
				} else {
					return oldRowSize;
				}
			}
			set{
				oldRowSize = value;
				EditorPrefs.SetInt ("sweetcli_rowSize", (int)Mathf.Max (0, value));
			}
		}
		#endregion
		#region style gui
		GUIStyle titleStyle;
		#endregion
		#region inspector message methods
		/// <summary>
		/// Called each time the object is selected
		/// </summary>
		void OnEnable(){
			//assign event for selecting a paletteItem

			PaletteWindow.PaletteItemSelectedEvent +=
				new PaletteWindow.paletteItemSelectedDelegate (UpdateCurrentPieceInstance);
			//target is a Editor "gameobject", the object being inspected.

			//esplicitly cast to target.
			targetLevel = (Level)target;
			oldRowSize = targetLevel.TotalRows;
			oldColumnSize = targetLevel.TotalColumns;
			sceneGuiTool = ScriptableObject.CreateInstance<LevelInspectorSceneGui>();

			InitiateStyle ();
			//incase OnEnable did nothing?
			if(ModeActions == null){
				ModeActions = new Dictionary<Mode,System.Action<int,int>>();
				ModeActions.Add( Mode.Painting, Paint	);
				ModeActions.Add( Mode.Delete,  Erase	);
				ModeActions.Add( Mode.Edit, Edit );
			}
			
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		void OnDisable(){
			//TODO save prefabList
			//TODO save renderedLevelGrid
		}
		void OnDestroy(){
			 PaletteWindow.PaletteItemSelectedEvent -=  new PaletteWindow.paletteItemSelectedDelegate(UpdateCurrentPieceInstance);
			prefabList.Clear ();
			renderedLevelGrid = null;
		}
		public override void OnInspectorGUI(){
			//DrawDefaultInspector ();
			InitLevel ();
			DrawDataValues();
			DrawSizingValues ();
			DrawPieceSelectedGUI();
			DrawInspectedItemGUI ();
			if (GUI.changed) {
				Undo.RecordObject(targetLevel, "Level Edit");
			}

		}
		#endregion

		#region Customs Methods
		private void InitLevel(){
			/*
			 * 
			 * PrefColumnSize = targetLevel.TotalColumns;
			 *	PrefRowSize = targetLevel.TotalColumns;
			 *	oldRowSize 
			 *	oldColumnSize
			 * 
			 */

			s_Object = new SerializedObject (targetLevel);
			//find the serialize field from level.cs
			s_TotalTimeProperty = s_Object.FindProperty ("_totalTime");
			//any object marked with hideFlags will not be editable.
			targetLevel.transform.hideFlags = HideFlags.NotEditable;
			if (prefabList == null ) {
				prefabList = new List <LevelPiece>();
			}
			if (renderedLevelGrid == null ) {
				targetLevel.LevelPieceGridPositions = new int[targetLevel.TotalColumns][];
				targetLevel.initiateRows ();
				renderedLevelGrid = targetLevel.LevelPieceGridPositions;
			}
//			if (prefabList != null ) {
//				targetLevel.LevelPieces = prefabList;
//			}
//			if (renderedLevelGrid != null ) {
//				targetLevel.LevelPieceGridPositions = renderedLevelGrid;
//			}
			//setLevel pieces based on size of array.
//			if (targetLevel.LevelPieces == null || targetLevel.LevelPieces.Count == 0) {
//				//get columns and row sizes by setting editorpref oldColumnSize and oldRowSize
//				//ints do nothing!
//				ResetLevelSize ();
//				targetLevel.LevelPieces = new List <LevelPiece> ();
//				prefabList = new List <LevelPiece>();
//			}
//			if (targetLevel.LevelPieceGridPositions == null) {
//				targetLevel.LevelPieceGridPositions = new int[targetLevel.TotalColumns][];
//				targetLevel.initiateRows ();
//				renderedLevelGrid = targetLevel.LevelPieceGridPositions;
//			}
			targetLevel.transform.hideFlags = HideFlags.NotEditable;
		}

		private void ResetLevelSize(){
			//resets the level to last configuration
			targetLevel.TotalRows = PrefRowSize;
			targetLevel.TotalColumns = PrefColumnSize;
		}

		void DrawDataValues(){
			//Label to mark first section
			EditorGUILayout.LabelField ("Data",titleStyle);
			//new box
			EditorGUILayout.BeginVertical ("box");
			//add custom property instead of intField
			EditorGUILayout.PropertyField (s_TotalTimeProperty);
			//save and disbale
			targetLevel.Settings = (LevelSettings)EditorGUILayout.ObjectField ("Level Settings",
				targetLevel.Settings, typeof(LevelSettings), false);
			if (targetLevel.Settings) {
				//create it 
				Editor.CreateEditor (targetLevel.Settings).OnInspectorGUI();
			} else {
				//use the existing(current) EditorGUI
				EditorGUILayout.HelpBox ("Please attach a Level Settings scriptableObject",MessageType.Warning);
			}

			EditorGUILayout.EndVertical();
//			targetLevel.Gravity = EditorGUILayout.FloatField ("Gravity",targetLevel.Gravity);
//			//using Mathf.Max to prevent negative numbers
//			//targetLevel.TotalTime = EditorGUILayout.IntField ("Total Time",Mathf.Max (0,targetLevel.TotalTime));
//
//			targetLevel.Bgm = (AudioClip)EditorGUILayout.ObjectField ("Bgm", targetLevel.Bgm, typeof(AudioClip), false);
//			targetLevel.Background = (Sprite)EditorGUILayout.ObjectField ("Background", targetLevel.Background, typeof(Sprite), false);
//
			targetLevel.SetGravity ();
		}


		//Resize level pieces and existing prefabs
		public void ResizeLevel(){
			//resize array based on new sizes with Temp array
			//resize the coordinate grid and initiate rows needed
			//copy old pieces to new list
			//update index of prefabs in new resized grid
			targetLevel.resizeCoordinateGrid();

			PrefColumnSize = targetLevel.TotalColumns;
			PrefRowSize = targetLevel.TotalRows;
		}

		void DrawSizingValues(int minValue = 10,int maxValue = 100){
			EditorGUILayout.LabelField ("Size", titleStyle);
			EditorGUILayout.BeginVertical ("box");
				//keep old size for copying prefabs
				int oldCols = targetLevel.TotalColumns;
				int oldRows = targetLevel.TotalRows;

				EditorGUILayout.BeginHorizontal ();
					//by binding logic to a layout box won't include the sliders
					//reassign values to the define drawing object on update of editor
					EditorGUILayout.PrefixLabel ("Column Size");
					targetLevel.TotalColumns = EditorGUILayout.IntSlider (targetLevel.TotalColumns, minValue, maxValue);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.PrefixLabel ("Row Size");
					targetLevel.TotalRows = EditorGUILayout.IntSlider (targetLevel.TotalRows, minValue, maxValue);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Space();




			EditorGUILayout.BeginHorizontal ();
				//state holder if GUI enabled.
				//brilliantly works local to EditorGUILayout.
				bool oldEnabled = GUI.enabled;
				EditorGUILayout.BeginVertical ();
					//should we enable the reset button as we use a slider for new values
					GUI.enabled = (targetLevel.TotalRows != PrefRowSize || targetLevel.TotalColumns != PrefColumnSize);

					bool buttonResize = GUILayout.Button ("Resize", GUILayout.Height (2 * EditorGUIUtility.singleLineHeight));
					if (buttonResize) {
						if(EditorUtility.DisplayDialog (
							"Resize Level",
							"Resize the level?\nadded prefabs may be lost.",
							"Yes",
							"No"
						)){
							ResizeLevel ();
						}
					}
					//displaye reset button
					bool buttonReset = GUILayout.Button ("Reset");
					if(buttonReset)  {
						if(EditorUtility.DisplayDialog (
							"Reset Level Size",
							"Are you sure you want to reset the level to previous size?\nadded prefabs may be lost.",
							"Yes",
							"No"
						)){
							ResetLevelSize ();
						}
					}
					//reset state
					GUI.enabled = oldEnabled;
				EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

		}

		private void UpdateCurrentPieceInstance(PaletteItem item, Texture2D preview){
			paletteItemSelected = item;
			itemPreview = preview;
			Repaint ();
		}

		void DrawPieceSelectedGUI(){
			EditorGUILayout.LabelField ("Selected Level Piece",titleStyle);
			if (paletteItemSelected == null) {
				EditorGUILayout.HelpBox ("No Level Piece Selected",MessageType.Info);
			} else {
				//follow at cursor position
				EditorGUILayout.BeginVertical ("box");
				//use two labels for a image with text underneath
				EditorGUILayout.LabelField (new GUIContent (itemPreview),GUILayout.Height(40));
				EditorGUILayout.LabelField (paletteItemSelected.itemName);
				EditorGUILayout.EndVertical ();
			}
		}

		void DrawInspectedItemGUI(){
			//TODO error on selection, lose focus,
			//TODO edit not working or this function
			//fires for Repaint() and ensure only for edit
			if (sceneGuiTool.CurrentMode != Mode.Edit) {
				return;
			}
			//design the inspector view for a edit piece
			EditorGUILayout.LabelField ("Piece Edited",  titleStyle);

			if(paletteItemInspected != null) {
				EditorGUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Name: " + paletteItemInspected.name);
				//creates a customer editor for target object, in this case a script
				Editor.CreateEditor( paletteItemInspected.inspectedScript).OnInspectorGUI();
				EditorGUILayout.EndVertical();
			} else {
				EditorGUILayout.HelpBox("No piece to edit!", MessageType.Info);
			}
		}
		/**
		* Unity Editor scene 'magic' function to extend drawing on to the screen
		*/
		void OnSceneGUI(){
			sceneGuiTool.DrawModeGui();
			if(sceneGuiTool.ModeHandler()){
				Repaint();
			}
			sceneGuiTool.EventHandler();
			//get the worldPoint and gridPoint from the Camera raycast to mouse position
			Vector3 worldPoint = Camera.current.ScreenToWorldPoint(sceneGuiTool.MousePoint);
			Vector3 gridPoint = targetLevel.WorldToGridCoordinates(worldPoint);
			//cast to int.
			int col = (int) gridPoint.x;
    		int row = (int) gridPoint.y;
			
			//We're handling our current mode and there was a mouseclick down  or drag perform an action.
			//use the EventType to correspond with the Event.current.type
			if(ModeActions.ContainsKey(sceneGuiTool.CurrentMode) &&
			(Event.current.type == EventType.MouseDown ||
			Event.current.type == EventType.MouseDrag )){
				originalPosX = col;
				originalPosY = row;
				ModeActions[sceneGuiTool.CurrentMode](col,row);
			}
			if(sceneGuiTool.CurrentMode == Mode.Edit && (Event.current.type == EventType.MouseUp || 
				Event.current.type == EventType.Ignore)){
				//try moving
				if(paletteItemInspected != null) {
					Move ();
				}
			}
			//enable freeMovement handle
			if (paletteItemInspected != null) {
				//from it's position enable the handle
				paletteItemInspected.transform.position =
					Handles.FreeMoveHandle (
					paletteItemInspected.transform.position,
					paletteItemInspected.transform.rotation,
					Level.GridSize / 2,
					Level.GridSize / 2 * Vector3.one,
					Handles.RectangleCap 
				);	
			}
			
		}		
		
		public void Paint(int col, int row){
			//out of bounds don't paint
			if(!targetLevel.IsInsideGridBounds(col,row)){
				return;
			}
			//paint over a piece
			DestroyLevelPiece(col,row);
			//paint new one 
			//TODO mark dirty when change done.
			EditorGUI.BeginChangeCheck();
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (target, "obj.name Added");
			}
			GameObject obj = PrefabUtility.InstantiatePrefab(paletteItemSelected.transform.gameObject) as GameObject;
			//set the new prefab as a child to the level
			obj.transform.parent = targetLevel.transform;
			obj.name = string.Format("[{0},{1}][{2}]", col, row, obj.name);
			obj.transform.position = targetLevel.GridToWorldCoordinates(col,row);
			obj.hideFlags = HideFlags.HideInHierarchy;
			var gamepiece = obj.GetComponent<LevelPiece> ();

			//add new prefab to level pieces array
			//targetLevel.setLevelPiece(col,row,gamepiece);

			prefabList.Add (gamepiece);
			renderedLevelGrid[col][row] = prefabList.IndexOf (gamepiece);


		}
		public void Edit(int col, int row){
			// we were general to get to this function, but don't want to handle drag when editing.
			if (targetLevel.IsInsideGridBounds (col, row)) {
				//in bounds..
				int cacheIndex = renderedLevelGrid [col] [row];
				LevelPiece validPiece = cacheIndex > -1 ? prefabList [cacheIndex] : null;
				if (validPiece != null) {
					paletteItemInspected = prefabList [cacheIndex].GetComponent <PaletteItem> () as PaletteItem;
				} 
			} else {
				paletteItemInspected = null;		
			}
			Repaint ();
		}
		//allows prefab moving in level designer.
		//only works in edit mode
		public void Move(){
			Vector3 gridPoint = targetLevel.WorldToGridCoordinates (paletteItemInspected.transform.position);
			int col = (int)gridPoint.x;
			int row = (int)gridPoint.y;
			//at same positon do nothing
			if(col == originalPosX && row == originalPosY) {
				return;
			}
			//continue;
			int index = renderedLevelGrid[col][row];
			LevelPiece validPiece = index > -1 ? prefabList [index] : null;
			//if in point or a valid prefab.
			if (!targetLevel.IsInsideGridBounds (col,row) || validPiece != null) {
				//return to original location
				paletteItemInspected.transform.position = targetLevel.GridToWorldCoordinates (originalPosX, originalPosY);
			} else {
				//move the prefab
				paletteItemInspected.transform.position = targetLevel.GridToWorldCoordinates (originalPosX, originalPosY);
				//update the jaggedArray
				renderedLevelGrid [originalPosX] [originalPosY] = -1;
				renderedLevelGrid [col] [row] = index;
				paletteItemInspected.transform.position = targetLevel.GridToWorldCoordinates(col,row);
			}

		}

		bool DestroyLevelPiece(int col, int row)
		{
//			LevelPiece validPiece = targetLevel.getLevelPiece (col, row);
//			int cacheIndex = targetLevel.LevelPieceGridPositions [col] [row];
			int cacheIndex = renderedLevelGrid [col] [row];
			LevelPiece validPiece = cacheIndex > -1 ? prefabList [cacheIndex] : null;
			if (validPiece != null  ) {
				//by destroying the gameObject at position
				//targetLevel.LevelPieces [cacheIndex] = null;
				//can't removeAT, doing so will delete index and deduct index, changing the key. as the key will be lowered. 
				prefabList [cacheIndex] = null;
				//targetLevel.LevelPieceGridPositions [col] [row] = -1;
				renderedLevelGrid [col] [row] = -1;
				DestroyImmediate (validPiece.transform.gameObject);
				return true;
			}
			return false;
		}

		public void Erase(int col,int row){
			if(!targetLevel.IsInsideGridBounds(col,row)){
				return;
			}
			//paint over a piece
			DestroyLevelPiece (col, row);
		}
		#endregion

		#region style functions
		void InitiateStyle(){
			GUISkin skin = (GUISkin)Resources.Load("Demo_GUISkin");
			titleStyle = skin.label;
//			titleStyle.alignment = TextAnchor.MiddleCenter;
//			titleStyle.fontSize = 16;

			//grab resource from the resource directory
			//not textures in editor must have the right type in their inspector window
//			Texture2D titleBg = (Texture2D)  
//				Resources.Load("Color_Bg");
//			Font titleFont = (Font) 
//				Resources.Load("Oswald-Regular");
			//.normal referes to the state. a label only has normal. but if there was a button, 
			//there would be a pressed state that could be styled a different way
//			titleStyle.normal.background = titleBg;
//			titleStyle.normal.textColor = Color.white;
//			titleStyle.font = titleFont;
		}
		#endregion
	}
}
