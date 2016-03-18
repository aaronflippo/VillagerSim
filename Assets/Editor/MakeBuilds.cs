using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
 
public class AutoBuilder {
 
	static string VERSION_STRING = "0.1";

	static string BaseGameName()
	{
		return  PlayerSettings.productName+"_"+VERSION_STRING;	
	}

	[MenuItem ("Make Build/Mac")]
	static void DoBuild_OSX(){
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneOSXIntel64 );

		string folder ="Builds/"+VERSION_STRING+"/"+BaseGameName()+"_MAC";
		MakeBuild( folder, PlayerSettings.productName+".app", BuildTarget.StandaloneOSXIntel64 );
	
		Debug.Log("Mac Build Complete!");

	}

	[MenuItem ("Make Build/Windows")]
	static void DoBuild_WIN(){
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneWindows );

		string folder ="Builds/"+VERSION_STRING+"/"+BaseGameName()+"_WIN";
		MakeBuild( folder, PlayerSettings.productName+".exe", BuildTarget.StandaloneWindows );

		//delete pdb files.

		string[] pdbFiles = Directory.GetFiles(folder,"*.pdb");
		foreach(string filename in pdbFiles)
		{
			File.Delete(filename);
		}

		Debug.Log("Window Build Complete!");

	}

	[MenuItem ("Make Build/Linux")]
	static void DoBuild_Linux(){	
		
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneLinux );

		string folder ="Builds/"+VERSION_STRING+"/"+BaseGameName()+"_LINUX";
		MakeBuild( folder, PlayerSettings.productName+".x86", BuildTarget.StandaloneLinux );


		Debug.Log("Linux Build Complete!");
	}


  
	[MenuItem ("Make Build/Windows+Mac+Linux")]
  static void BuildAll()
  {
		DoBuild_OSX();
		DoBuild_WIN();
		DoBuild_Linux();

		Debug.Log("Finished builds");


	} 
	
	static public void MakeBuild ( string path, string exeName, BuildTarget target)
	{
		Debug.Log("making build "+exeName+" at path: "+path);

		if(Directory.Exists(path))
			Directory.Delete(path,true);
		
		Directory.CreateDirectory(path);
			
		//string [] scenes = { "Assets/TheWorld.unity", "Assets/Worlds/Basic.unity", "Assets/Worlds/Apocalypse.unity", "Assets/Worlds/Void.unity", "Assets/Worlds/Labyrinth.unity", "Assets/Worlds/Zen.unity", "Assets/Worlds/Credits.unity"};

		//List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

		List<string> enabledScenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in scenes)
		{
			if (scene.enabled)
			{
				enabledScenes.Add(scene.path);
			}
		}



		BuildPipeline.BuildPlayer(enabledScenes.ToArray(), Path.Combine(path,exeName), target, BuildOptions.ShowBuiltPlayer );		
	}
}
