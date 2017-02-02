using UnityEditor;

namespace AppBuilder{
public class BuildSettings {

	public static void UpdateSettings(){
		PlayerSettings.companyName = "Green Farm Games";
		PlayerSettings.productName = "Editor Scripting Demo";
		PlayerSettings.bundleVersion = "1.0.0";

		//ANDROID AND IOS
		PlayerSettings.bundleIdentifier = "com.greenfarmgames.editorscriptingdemo";

	}
}
}
