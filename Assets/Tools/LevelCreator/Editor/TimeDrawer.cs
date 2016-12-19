using UnityEngine;
using UnityEditor;

namespace sweetcli.LevelCreator {
	[CustomPropertyDrawer(typeof (sweetcli.LevelCreator.TimeAttribute))]
	public class TimeDrawer : PropertyDrawer {
		//Assumed didn't have to alter the GetProperyHeight and OnGui, but they didn't render correctly first time.
		//test 1, just altering the ONGui and defining a RECT
		//have to override GetProperyheight
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight (property, label) * 2;
		}
		//have to override OnGUI for rendering
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Integer) {
				//draw the intValue based on a intField but div height by 2.
				property.intValue = EditorGUI.IntField (new Rect (position.x, position.y, position.width, position.height / 2), 
					label, Mathf.Max (0, property.intValue)); //copy this for the label
				//postion of the 'formatted' text.
				// positioning not explained will in the book.
				EditorGUI.LabelField (new Rect (position.x, position.y + position.height / 2, position.width, position.height / 2), " ", TimeFormat (property.intValue));

			} else {
				EditorGUI.HelpBox (position, "To use the Time attribute \"" + label.text + "\" must be an int!", MessageType.Error);
			}
			//there are no layout gui so must use a rect if any layout is to be achieved.
			//base.OnGUI (position, property, label);
		}
		#region custom method
		public string TimeFormat(int totalSeconds){
			TimeAttribute time = attribute as TimeAttribute;
			string timeFormat;
			//calculate time base on hours shown or not
			if(time.showHours){
			int hours = totalSeconds / (60 * 60);
			int minutes = (totalSeconds % (60 * 60))/60;
			int seconds = totalSeconds % 60; // mod to flatten given` seconds
				timeFormat = string.Format ("{0}:{1}:{2} (h:m:s)",
												hours,
												minutes.ToString ().PadLeft (2, '0'), 
												seconds.ToString ().PadLeft (2, '0')
											);
			}else{
				int minutes = totalSeconds /60;
				int seconds = totalSeconds % 60; // mod to flatten given` seconds
				//c# string.Format
				timeFormat = string.Format ("{0}:{1} (m:s)", 
											minutes.ToString (), 
											seconds.ToString ().PadLeft (2, '0'));
			}

			return timeFormat;

		}
		#endregion
		
	}
}