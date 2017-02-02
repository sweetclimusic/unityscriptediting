using UnityEngine;
using System.Collections;

namespace RunAndJump {
	//expandable player settings
	[System.Serializable]
	public class LevelSettings : ScriptableObject
	{
		public float gravity = -30;
		public AudioClip bgm;
		public Sprite background;
	}
}
