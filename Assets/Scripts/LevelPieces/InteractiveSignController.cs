using UnityEngine;
using System.Collections;
using sweetcli.LevelCreator;

namespace RunAndJump {

	public class InteractiveSignController : LevelPiece {

		#if UNITY_EDITOR
		//scriptable object to place a item
		public sweetcli.LevelCreator.PaletteItem paletteItem;
		#endif

		public AudioClip SignFx;
		public string Message;

		public delegate void StartInteractionDelegate(string message);
		public static event StartInteractionDelegate StartInteractionEvent;

		public delegate void StopInteractionDelegate();
		public static event StopInteractionDelegate StopInteractionEvent;

		private void OnTriggerEnter2D(Collider2D col) {
			if(StartInteractionEvent != null) {
				AudioPlayer.Instance.PlaySfx (SignFx);
				StartInteractionEvent(Message);
			}
		}

		private void OnTriggerExit2D(Collider2D col) {
			if(StopInteractionEvent != null) {
				StopInteractionEvent();
			}

		}
	}
}
