using UnityEngine;

public class GizmoExample : MonoBehaviour {

	private void OnDrawGizmos(){
		Gizmos.color = Color.white;
		//attache to a gameobject to display the gizmo
		//not practical but its a demo.
		Gizmos.DrawCube (transform.position,Vector3.one);
	}

	private void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position,Vector3.one);
	}
}
