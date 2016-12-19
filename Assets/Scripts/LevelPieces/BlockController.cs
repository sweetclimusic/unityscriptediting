using UnityEngine;
using System.Collections;
using sweetcli.LevelCreator;

namespace RunAndJump {
	public class BlockController : LevelPiece {
		#if UNITY_EDITOR
		//scriptable object to place a item
		public PaletteItem paletteItem;
		#endif
	}
}
