using UnityEngine;
using UnityEditor;
using System.Collections;
using RunAndJump;
using System;

namespace sweetcli.LevelCreator {
	//overrite the inspector for RunAndJump.Level when ever there is a level.
	[CustomEditor(typeof(RunAndJump.Level))]
	public class LevelInspector : Editor{
		//instance of the current level
		private Level targetLevel;

		//storage
		private int oldColumnSize;
		private int oldRowSize;
		private int minValue;
		private SerializedObject s_Object;
		private SerializedProperty s_TotalTimeProperty;

		#region prefab selection event
		private PaletteItem paletteItemSelected;
		private Texture2D itemPreview;
		private LevelPiece prefabSelected;
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
				EditorPrefs.SetInt ("sweetcli_rowSize", (int)Mathf.Max (0, value) );
			}
		}
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
			InitLevel ();
			//sets editorpref oldColumnSize and oldRowSize
			ResetLevelSize (oldColumnSize,oldRowSize);
		}
		/// <summary>
		/// Raises the disable event.
		/// </summary>
		void OnDisable(){
		}
		void OnDestroy(){
			 PaletteWindow.PaletteItemSelectedEvent -=  new PaletteWindow.paletteItemSelectedDelegate(UpdateCurrentPieceInstance);
		}
		public override void OnInspectorGUI(){
			//DrawDefaultInspector ();
			DrawDataValues();
			DrawSizingValues ();
			DrawPieceSelectedGUI();

		}
		#endregion

		#region Custome Methods
		private void InitLevel(){
			s_Object = new SerializedObject (targetLevel);
			//find the serialize field from level.cs
			s_TotalTimeProperty = s_Object.FindProperty ("_totalTime");
			//setLevel pieces based on size of array.
			if (targetLevel.LevelPieces == null || targetLevel.LevelPieces.Length == 0) {
				targetLevel.LevelPieces = new LevelPiece[ targetLevel.TotalColumns * targetLevel.TotalRows];
			}
		}

		private void ResetLevelSize(int oldColSize, int oldRowSize){
			//resets the level to last configuration
			targetLevel.TotalRows = PrefRowSize;
			targetLevel.TotalColumns = PrefColumnSize;

		}

		void DrawDataValues(){
			//Label to mark first section
			EditorGUILayout.LabelField ("Data",EditorStyles.boldLabel);
			//add custom property instead of intField
			EditorGUILayout.PropertyField (s_TotalTimeProperty);
			targetLevel.Gravity = EditorGUILayout.FloatField ("Gravity",targetLevel.Gravity);
			//using Mathf.Max to prevent negative numbers
			//targetLevel.TotalTime = EditorGUILayout.IntField ("Total Time",Mathf.Max (0,targetLevel.TotalTime));

			targetLevel.Bgm = (AudioClip)EditorGUILayout.ObjectField ("Bgm", targetLevel.Bgm, typeof(AudioClip), false);
			targetLevel.Background = (Sprite)EditorGUILayout.ObjectField ("Background", targetLevel.Background, typeof(Sprite), false);
			
		}


		//Resize level pieces and existing prefabs
		public void ResizeLevel(){
			//resize array based on new sizes with Temp array
				LevelPiece[] newPieceObjectContainers = new LevelPiece[ targetLevel.TotalColumns * targetLevel.TotalRows];
			bool smallerLevel = (bool)(newPieceObjectContainers.Length > targetLevel.LevelPieces.Length);

			//get existing pieces into the newPieces array
			for (int col = 0; col < oldColumnSize; col++) {
				for (int row = 0; row < oldRowSize; row++) {
					//assign old prefab to new container.
					if (col < targetLevel.TotalColumns && row < targetLevel.TotalRows) {
							newPieceObjectContainers [col + row * targetLevel.TotalColumns] =
							targetLevel.LevelPieces [col + row * oldColumnSize];
					}
					else {
					LevelPiece piece = null;
					int index = col + row * oldColumnSize;
					//stay in boundries for array copying
						if (smallerLevel){
							//what is smaller?
							//columns, rows, both.
							Debug.Log("current col" + col);
							Debug.Log("current row" + row);
							Debug.Log(newPieceObjectContainers.Length);
							Debug.Log(targetLevel.LevelPieces.Length);
						} 

						//grab the prefab and destroy
						//TODO goes out of range when shrinks
						//	if (  targetLevel.LevelPieces.Length > index ) {
								
						//	}
						piece = targetLevel.LevelPieces [index];
						if (piece != null ) {
							// we must to use DestroyImmediate in a Editor context
							UnityEngine.Object.DestroyImmediate (piece.gameObject);
						}
					}

				}	//end row loop
			} //end col loop
			//reset arrays.
			targetLevel.LevelPieces = newPieceObjectContainers;
			PrefColumnSize = targetLevel.TotalColumns;
			PrefRowSize = targetLevel.TotalRows;
		}

		void DrawSizingValues(int minValue = 10,int maxValue = 100){
			EditorGUILayout.LabelField ("Size", EditorStyles.boldLabel);
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
							ResetLevelSize (oldCols,oldRows);
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
			prefabSelected = (LevelPiece)item.GetComponent <LevelPiece> ();
			Repaint ();
		}

		void DrawPieceSelectedGUI(){
			EditorGUILayout.LabelField ("Selected Level Piece");
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
		#endregion
	}
}
