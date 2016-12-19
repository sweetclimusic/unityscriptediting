using UnityEngine;

namespace sweetcli.LevelCreator{
	/// <summary>
	/// Time attribute.
	/// Basic property for a custom ProperyDraw.
	/// </summary>
	public class TimeAttribute : PropertyAttribute {
		public readonly bool showHours;

		public TimeAttribute( bool displayHours = false ){
				showHours = displayHours;
		}
	}	
}