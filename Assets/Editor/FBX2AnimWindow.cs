using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
 
public class FBX2AnimWindow : EditorWindow
{
    public static string assetSrcFolderPath = "Assets/Res/Unit/Characters/Vopi/FBX";
    public static string assetDstFolderPath = "Assets/Res/Unit/Characters/Vopi/Animations";
    [MenuItem ("Tools/Animation/FBX中提取animationclip")]
    public static void ShowWindow () {
        EditorWindow thisWindow = EditorWindow.GetWindow(typeof(FBX2AnimWindow));
        thisWindow.titleContent = new GUIContent("fbx动画资源提取");
        thisWindow.position = new Rect(Screen.width/2, Screen.height/2, 600, 200);
    }
 
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择源文件夹");
        EditorGUILayout.TextField(assetSrcFolderPath);
        if (GUILayout.Button("选择"))
        {
            assetSrcFolderPath = EditorUtility.OpenFolderPanel("选择文件夹", assetSrcFolderPath, "");
        }
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择目标文件夹");
        EditorGUILayout.TextField(assetDstFolderPath);
        if (GUILayout.Button("选择"))
        {
            assetDstFolderPath = EditorUtility.OpenFolderPanel("选择文件夹", assetDstFolderPath, "");
        }
        EditorGUILayout.EndHorizontal();
 
        if (GUILayout.Button("开始提取") && assetSrcFolderPath != null && assetDstFolderPath != null)
        {
            Seperate();
        }
    }
    private static void Seperate(){
        assetSrcFolderPath = PathTools.GetAbsolutePath(assetSrcFolderPath);//先获取这个文件路径的绝对路径
        var files = Directory.GetFiles(assetSrcFolderPath, "*.FBX");
        string dstPath = PathTools.GetRelativePath(assetDstFolderPath);
        foreach (var file in files)
        {
            string srcPath = PathTools.GetRelativePath(file);
            AnimationClip srcclip = AssetDatabase.LoadAssetAtPath(srcPath, typeof(AnimationClip)) as AnimationClip;
            if (srcclip == null)
                continue;
 
            //本身有这个文件的话就删掉
            AnimationClip dstclip = AssetDatabase.LoadAssetAtPath(dstPath, typeof(AnimationClip)) as AnimationClip;
            if (dstclip != null)
                AssetDatabase.DeleteAsset(dstPath);
 
            AnimationClip tempclip = new AnimationClip();
            EditorUtility.CopySerialized(srcclip, tempclip);
            AssetDatabase.CreateAsset(tempclip, dstPath + "/" + srcclip.name + ".anim");
        }
    }
    
    
    
}
 
public class PathTools
{
    public static string GetRelativePath(string path){
        string srcPath = path.Replace("\\", "/");
        var retPath =  Regex.Replace(srcPath, @"\b.*Assets", "Assets");
        return retPath;
    }
    public static string GetAbsolutePath(string path){
        string srcPath = path.Replace("\\", "/");
        var retPath =  Regex.Replace(srcPath, @"\b.*Assets", Application.dataPath);
        return retPath;
    }
}