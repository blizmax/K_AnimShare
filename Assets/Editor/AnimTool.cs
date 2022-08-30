using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public static class AnimTool
{
    [MenuItem("尼尔工具箱/重新命名文件（十六进制转十进制）")]
    public static void RenameFile()
    {
        string _path =
            EditorUtility.SaveFolderPanel("选择文件夹", "F:/GameModelExtract/NierExtract/data006_files/pl/pl000f/", "");
        string _newPath = _path + "_new";
        if (!Directory.Exists(_newPath))
        {
            Directory.CreateDirectory(_newPath);
        }

        string[] _files = System.IO.Directory.GetFiles(_path);
        foreach (var _filePath in _files)
        {
            var _extension = Path.GetExtension(_filePath);
            var _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            var _newFileName = _fileNameWithoutExtension;
            string[] _tmp = _fileNameWithoutExtension.Split('_');
            if (_tmp.Length >= 2)
            {
                //进制转换
                int _id = Convert.ToInt32(_tmp[1], 16);
                //保留五位数，列如：00001
                var _format = string.Format("{0:d5}", _id);
                _tmp[1] = _format;
                _newFileName = _tmp[0];
                for (int i = 1; i < _tmp.Length; i++)
                {
                    _newFileName += "_" + _tmp[i];
                }

                var _srcfileName = _newPath + "/" + _newFileName + _extension;
                File.Copy(_filePath, _srcfileName);
            }
            else
            {
                var _srcfileName = _newPath + "/" + _newFileName + _extension;
                File.Copy(_filePath, _srcfileName);
            }
        }

        EditorUtility.DisplayDialog("", "处理完成！", "OK");
    }

    [MenuItem("尼尔工具箱/解析mot文件生成数据")]
    public static void ReadMotFile()
    {
        string _path =
            EditorUtility.SaveFolderPanel("选择文件夹",
                "F:/GameModelExtract/GameFile/NierExtract/data006_files/pl/pl000f_new", "");
        var _directoryName = Path.GetFileName(_path);
        int _firstFrame = 1;
        int _lastFrame = _firstFrame - 1;
        List<string> _list = new List<string>();
        string[] _files = System.IO.Directory.GetFiles(_path);
        foreach (var _filePath in _files)
        {
            var _extension = Path.GetExtension(_filePath);
            var _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            /*
             mot文件头结构
             struct {
                    char      id[4]; // "mot\0"   1
                    uint32    hash;  4
                    uint16    flag;  4
                    int16     frameCount;  2
                    uint32    recordOffset;  4
                    uint32    recordNumber; 4
                    uint32    unknown; // usually 0 or 0x003c0000, maybe two uint16  4
                    string    animName; // found at most 12 bytes with terminating 0
                } 
             */
            if (_extension.Equals(".mot"))
            {
                var _fByte = File.ReadAllBytes(_filePath);

                string AnimID = Path.GetFileName(_filePath);

                //读取动画片段总帧数
                var _frameCount = BitConverter.ToInt16(_fByte, 10);
                _lastFrame += _frameCount;
                _list.Add($"{_firstFrame}|{_lastFrame}|{_fileNameWithoutExtension}|{_frameCount}|{AnimID}");
                _firstFrame += _frameCount;
            }
        }

        File.WriteAllLines(_path + $"/{_directoryName}.txt", _list);
        EditorUtility.DisplayDialog("", "解析成功！", "OK");
    }


    /// <summary>
    /// Clip数据
    /// </summary>
    public class FbxClipData
    {
        public string mName;
        public int mFirstFrame;
        public int mLastFrame;
    }


    [MenuItem("尼尔工具箱/自动拆分FBX动画片段")]
    public static void SplitFbxClip()
    {
        if (EditorUtility.DisplayDialog("自动拆分FBX动画片段", "确认是否已在Project窗口下选中了要处理的FBX", "确定", "取消"))
        {
            UnityEngine.Object[] _objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            int _count = 0;
            int _objectsLength = _objects.Length;
            foreach (var obj in _objects)
            {
                _count++;
                string _assetPath = AssetDatabase.GetAssetPath(obj);
                if (Path.GetExtension(_assetPath)?.ToLower() != ".fbx")
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("正在处理,请稍后", _assetPath, (float)(_count) / _objectsLength);
                List<FbxClipData> _fbxClipDataList = new List<FbxClipData>();

                string _extension = Path.ChangeExtension(_assetPath, "txt");
                if (!File.Exists(_extension))
                {
                    Debug.LogError(_extension + " 数据文件不存在！");
                    continue;
                }

                if (string.IsNullOrEmpty(_extension))
                {
                    continue;
                }

                StreamReader _reader = new StreamReader(_extension);
                string _line;
                while ((_line = _reader.ReadLine()) != null)
                {
                    string[] _info = _line.Split('|');
                    FbxClipData _fbxClipData = new FbxClipData
                    {
                        mFirstFrame = int.Parse(_info[0]),
                        mLastFrame = int.Parse(_info[1]),
                        mName = _info[2]
                    };
                    _fbxClipDataList.Add(_fbxClipData);
                }

                _reader.Close();

                ModelImporter _modelImporter = AssetImporter.GetAtPath(_assetPath) as ModelImporter;
                ArrayList _clipsList = new ArrayList();
                for (int i = 0; i < _fbxClipDataList.Count; i++)
                {
                    ModelImporterClipAnimation _clipAnimation = new ModelImporterClipAnimation
                    {
                        name = _fbxClipDataList[i].mName,
                        firstFrame = _fbxClipDataList[i].mFirstFrame,
                        lastFrame = _fbxClipDataList[i].mLastFrame
                    };
                    _clipsList.Add(_clipAnimation);
                }

                if (_modelImporter != null)
                {
                    _modelImporter.clipAnimations =
                        (ModelImporterClipAnimation[])_clipsList.ToArray(typeof(ModelImporterClipAnimation));
                    _modelImporter.SaveAndReimport();
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "处理完成", "OK");
        }
    }


    [MenuItem("尼尔工具箱/初始化动画片段txt文件")]
    public static void InitAnimClip()
    {
        if (EditorUtility.DisplayDialog("初始化动画片段txt文件", "确认是否已在Project窗口下选中了要处理的动画片段txt文件", "确定", "取消"))
        {
            UnityEngine.Object[] _objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            int _count = 0;
            int _objectsLength = _objects.Length;

            

            foreach (var obj in _objects)
            {
                List<string> _list = new List<string>();
                int? intOffset = null;
                
                string _assetPath = AssetDatabase.GetAssetPath(obj);

                StreamReader _reader = new StreamReader(_assetPath);
                string _line;
                var _directoryName = Path.GetFileName(_assetPath);
                Debug.Log(_directoryName);


                while ((_line = _reader.ReadLine()) != null)
                {
                    string[] _info = _line.Split('|');

                    if (intOffset == null)
                        intOffset = int.Parse(_info[0]);

                    string[] _info_new = _info;
                    _info_new[0] = (int.Parse(_info[0]) - intOffset + 1).ToString();
                    _info_new[1] = (int.Parse(_info[1]) - intOffset + 1).ToString();

                    string str = "";
                    for (int i = 0; i < _info_new.Length; i++)
                    {
                        str += _info_new[i];

                        if (i < _info_new.Length - 1)
                            str += "|";
                    }

                    _list.Add(str);
                }

                _reader.Close();

                File.WriteAllLines(_assetPath.Replace(_directoryName, "") + $"{_directoryName}_new.txt", _list);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "处理完成", "OK");
        }
    }
}