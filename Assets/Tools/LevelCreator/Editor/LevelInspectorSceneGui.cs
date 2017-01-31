using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using RunAndJump.LevelPackager;

namespace sweetcli.LevelCreator
{
	public class LevelInspectorSceneGui : ScriptableObject {
		#region mode selection
		private Mode selectedMode;
		private Mode currentMode;
		//check the keyExist.
		public Mode CurrentMode{
			get {
				if (EditorPrefs.HasKey ("sweetcli_currentMode")) {
					
					return (Mode)EditorPrefs.GetInt ("sweetcli_currentMode"); 
				} else {
					return currentMode;
				}
			}
			set{
				currentMode = value;
				EditorPrefs.SetInt ("sweetcli_currentMode", (int)Mathf.Max (0, (int)value));
			}
		}
		public Mode SelectedMode{
			get{
				return selectedMode;
			}
			set{
				selectedMode = value;
				//disable unity transform tools for Painting.Edit,Delete
				if(selectedMode == Mode.View){
					Tools.current = Tool.View;
				}else{
					Tools.current = Tool.None;
				}
			}
		}

		private Vector3 mousePoint;
		public Vector3 MousePoint{
			get{
				return mousePoint;
			}set{
				mousePoint = value;
			}
		}
		public bool ModeHandler(){
			bool modeUpdated = false;
			//detect a change to the mode selected
			if(SelectedMode != CurrentMode){
				//TODO, do more than update mode selected.
				CurrentMode = SelectedMode;
				modeUpdated = true;
			}
			
			//force to 2D view Mode.
			SceneView.currentDrawingSceneView.in2DMode = true;
			return modeUpdated;
		}
		
		public void EventHandler(){
			HandleUtility.AddDefaultControl(
				//take control of all events from setting this.
				GUIUtility.GetControlID(FocusType.Passive)
			);
				//get mouse from the Unity Event.
			MousePoint = new Vector3(Event.current.mousePosition.x,
			Camera.current.pixelHeight - Event.current.mousePosition.y,
			0);	
			
			//return Event.current.mousePosition;
		}

		#endregion

		public void DrawModeGui(){
			//use utility to fetch the Enums to a list.
			List<Mode> availableModes = EditorUtils.GetListFromEnum<Mode>();
			//get all labele from enum...
			List<string> modeLabels = new List<string>();
			foreach (Mode item in availableModes)
			{
				modeLabels.Add(item.ToString());
			}
			//Draw to Scene View
			Handles.BeginGUI();
				//rect are x, y, width, height

				GUILayout.BeginArea(new Rect(10f, 10f, 360f, 40f));
					SelectedMode = (Mode) GUILayout.Toolbar(
						(int) CurrentMode,
						modeLabels.ToArray(),
						GUILayout.ExpandHeight(true)
					);
				GUILayout.EndArea();
				
			Handles.EndGUI();
		}
	
	}
}