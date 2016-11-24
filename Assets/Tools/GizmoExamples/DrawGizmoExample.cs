using UnityEngine;
using UnityEditor;

//class to simulate the onDrawGizmo in UnityEngine
public class DrawGizmoExample  {
	//emulate onDrawGizmo, by making a method private static with 2 paramaters
	//these are flags that specify scenarios, in which the gizmos will be rendered and their behavior.
	//Is this a type of Unity override?
	[DrawGizmo(GizmoType.NotInSelectionHierarchy | 
		GizmoType.InSelectionHierarchy | 
		GizmoType.Selected | 
		GizmoType.Active | 
		GizmoType.Pickable)]
	private static void CustomGizmoDrawing(TargetExample target, GizmoType gizmotype){
		Gizmos.color = Color.white;
		Gizmos.DrawCube (target.transform.position, Vector3.one);
	}

	[DrawGizmo(GizmoType.InSelectionHierarchy | 
		GizmoType.Active
	)]
	//emulating OnDrawSelected
	private static void CustomGizmoSelectedDrawing(TargetExample target, GizmoType gizmotype){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (target.transform.position, Vector3.one);
	}

}
