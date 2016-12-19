using UnityEngine;
using System.Collections;
using sweetcli.LevelCreator;

namespace RunAndJump {

	public class HazardSpikesController : LevelPiece {

		public AudioClip PlayerLoseFx;
		#if UNITY_EDITOR
		//scriptable object to place a item
		public PaletteItem paletteItem;
		#endif
		private void OnCollisionEnter2D(Collision2D coll) {
			if (coll.gameObject.layer == LayerMask.NameToLayer("Player")) {
				PlayerController player = coll.gameObject.GetComponent<PlayerController>();
				AudioPlayer.Instance.PlaySfx (PlayerLoseFx);
				player.StartPlayerDeath();
			}
		}
	}
}
