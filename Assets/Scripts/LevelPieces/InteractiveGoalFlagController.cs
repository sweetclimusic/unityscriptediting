using UnityEngine;
using System.Collections;
using sweetcli.LevelCreator;

namespace RunAndJump {
	
	public class InteractiveGoalFlagController : LevelPiece { 

		#if UNITY_EDITOR
		//scriptable object to place a item
		public sweetcli.LevelCreator.PaletteItem paletteItem;
		#endif

		public AudioClip PlayerWinFx;
		
		public delegate void StartInteractionDelegate();
		public static event StartInteractionDelegate StartInteractionEvent;
		
		private void OnTriggerEnter2D(Collider2D col) {
			if(StartInteractionEvent != null) {
				AudioPlayer.Instance.PlaySfx (PlayerWinFx);
				StartInteractionEvent();
			}
		}
		
	}
	
}
