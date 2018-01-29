using UnityEditor;
using UnityEngine;
using Echo;

public class EchoEditor : EditorWindow {

	string inputFilePath="",mediaFilePath="",outputFolderPath="",customFileName="";
	GUIStyle browseFileStyle;
	bool options,generating;
	Color defaultGUIColor;
	float labelW = 50f;
	float browseButtonW = 50f;

	[MenuItem("Window/ECHO")]
	public static void ShowWindow(){
		EditorWindow.GetWindow(typeof(EchoEditor));
	}

	void OnGUI () {
		defaultGUIColor = GUI.color;
		title="ECHO";
		browseFileStyle=EditorStyles.textField;
		browseFileStyle.alignment = TextAnchor.MiddleRight;
		GUILayout.Label ("Files", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label(".TXT",GUILayout.Width(labelW));
		if(GUILayout.Button(inputFilePath,browseFileStyle,GUILayout.Width(position.width*.5f))){
			GUI.FocusControl("BrowseTXT");
			inputFilePath = BrowseForFile("txt");
		}
		
		GUI.SetNextControlName("BrowseTXT");
		
		if(GUILayout.Button("...",GUILayout.Width(browseButtonW))){ 
			GUI.FocusControl("BrowseTXT");
			inputFilePath = BrowseForFile("txt");
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(".FEV",GUILayout.Width(labelW));
		if(GUILayout.Button(mediaFilePath,browseFileStyle,GUILayout.Width(position.width*.5f))){
			GUI.FocusControl("BrowseFEV");
			mediaFilePath = BrowseForFile("fev");
		}
		
		GUI.SetNextControlName("BrowseFEV");
		
		if(GUILayout.Button("...",GUILayout.Width(browseButtonW))){ 
			GUI.FocusControl("BrowseFEV");
			mediaFilePath = BrowseForFile("fev");
		}
		GUILayout.EndHorizontal();
		options = EditorGUILayout.BeginToggleGroup ("Advanced Options", options);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Filename");
		customFileName = GUILayout.TextField(customFileName,GUILayout.Width(position.width*.75f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Destination");
		if(GUILayout.Button(outputFolderPath,browseFileStyle,GUILayout.Width(position.width*.5f))){
			GUI.FocusControl("BrowseDestination");
			outputFolderPath = BrowseForFolder();
		}
		GUI.SetNextControlName("BrowseDestination");
		if(GUILayout.Button("...",GUILayout.Width(browseButtonW))){ 
			GUI.FocusControl("BrowseDestination");
			outputFolderPath = BrowseForFolder();
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndToggleGroup ();
		GUILayout.BeginHorizontal();
		GUI.enabled = !options ? inputFilePath!=""&&mediaFilePath!="" : inputFilePath!=""&&mediaFilePath!=""&&customFileName!=""&&outputFolderPath!="";
		GUI.color = new Color(.4f,.9f,.4f);
		if(GUILayout.Button("Generate")){
			if(options){
				if(customFileName!=""){
					if(outputFolderPath!=""){
						EchoConverter.Convert(inputFilePath,mediaFilePath,customFileName,outputFolderPath);
					}else{
						EchoConverter.Convert(inputFilePath,mediaFilePath,customFileName);
					}
				}
			}else{
				EchoConverter.Convert(inputFilePath,mediaFilePath);
			}
		}
		GUI.enabled = inputFilePath!=""||mediaFilePath!=""||customFileName!=""||outputFolderPath!="";
		GUI.color = new Color(.9f,.4f,.4f);
		if(GUILayout.Button("Clear")){
			Clear();
		}
		GUI.color = defaultGUIColor;
		GUI.enabled=true;
		GUILayout.EndHorizontal();
		if(generating){
			EditorUtility.DisplayProgressBar("Generating","info",0);
		}else{
			EditorUtility.ClearProgressBar();
		}
	}

	string BrowseForFile(string extension){
		return EditorUtility.OpenFilePanel("Select File","Assets",extension);
	}

	string BrowseForFolder(){
		return EditorUtility.OpenFolderPanel("Select Folder","Assets","");
	}

	void Clear(){
		inputFilePath=mediaFilePath=outputFolderPath=customFileName="";
	}

	void OnEchoStarted(){
		Debug.Log("ECHO started!");
		generating=true;
	}

	void OnEchoCompleted(){
		Debug.Log("ECHO completed!");
		AssetDatabase.Refresh();
		generating=false;
	}

	void OnEchoFailed(string reason){
		Debug.LogError("ECHO failed: "+reason);
		generating=false;
	}

	void OnEnable(){
		EchoConverter.OnEchoStarted += OnEchoStarted;
		EchoConverter.OnEchoCompleted += OnEchoCompleted;
		EchoConverter.OnEchoFailed += OnEchoFailed;
	}

	void OnDisable(){
		EchoConverter.OnEchoStarted -= OnEchoStarted;
		EchoConverter.OnEchoCompleted -= OnEchoCompleted;
		EchoConverter.OnEchoFailed -= OnEchoFailed;
	}

	void OnDestroy(){
		EchoConverter.OnEchoStarted -= OnEchoStarted;
		EchoConverter.OnEchoCompleted -= OnEchoCompleted;
		EchoConverter.OnEchoFailed -= OnEchoFailed;
	}

}
