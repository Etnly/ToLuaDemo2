using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.IO;
using System.Reflection;


public class GameManager : Base
{
    private List<string> downloadFiles = new List<string>();

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    void Awake()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {

        CheckExtractResource(); //释放资源
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        bool isExists = Directory.Exists(Tools.DataPath) &&
          Directory.Exists(Tools.DataPath + "Lua/") && File.Exists(Tools.DataPath + "files.txt");
        if (isExists || AppConst.DebugMode)
        {
            StartCoroutine(OnUpdateResource());
            return;   //文件已经解压过了，自己可添加检查文件列表逻辑
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        string dataPath = Tools.DataPath;  //数据目录
        string resPath = Tools.AppContentPath(); //游戏包资源目录

        if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string infile = resPath + "files.txt";
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);

        Debug.Log(infile);
        Debug.Log(outfile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = resPath + fs[0];  //
            outfile = dataPath + fs[0];

            Debug.Log("正在解包文件:>" + infile);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.1f);

        //释放完成，开始启动更新资源
        StartCoroutine(OnUpdateResource());
    }

    IEnumerator OnUpdateResource()
    {
        string dataPath = Tools.DataPath;  //数据目录
        string url = AppConst.WebUrl;
        string message = string.Empty;
        string listUrl = url + "files.txt";
        Debug.LogWarning("LoadUpdate---->>>" + listUrl);
                                     
        WWW www = new WWW(listUrl);
        yield return www;
        if (www.error != null)
        {
            OnUpdateFailed(string.Empty);
            yield break;
        }
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        File.WriteAllBytes(dataPath + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');

        for (int i = 0; i < files.Length; i++)
        {
            if (string.IsNullOrEmpty(files[i])) continue;
            string[] keyValue = files[i].Split('|');
            string f = keyValue[0];
            string localfile = (dataPath + f).Trim();
            string path = Path.GetDirectoryName(localfile);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileUrl = url + f;
            bool canUpdate = !File.Exists(localfile);
            if (!canUpdate)
            {
                string remoteMd5 = keyValue[1].Trim();
                string localMd5 = Tools.md5file(localfile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate) File.Delete(localfile);
            }
            if (canUpdate)
            {   //本地缺少文件
                Debug.Log(fileUrl);
                message = "downloading>>" + fileUrl;
                print(message);

                www = new WWW(fileUrl);
                yield return www;
                if (www.error != null)
                {
                    OnUpdateFailed(path);   //
                    yield break;
                }
                File.WriteAllBytes(localfile, www.bytes);

            }
        }
        yield return new WaitForEndOfFrame();

        message = "更新完成!!";
        print(message);

        OnResourceInited();
    }
    void OnUpdateFailed(string file)
    {
        string message = "更新失败!>" + file;
        print(message);
    }


    /// <summary>
    /// 资源初始化结束
    /// </summary>
    public void OnResourceInited()
    {
        ResManager.Initialize(AppConst.AssetDir, delegate() {
            Debug.Log("Initialize OK!!!");
            this.OnInitialize();
        });
//        ResManager.Initialize();
//        Debug.Log("Initialize OK !!");
    }

    void OnInitialize()
    {
        
        LuaManager.Instance.LuaClient.luaState.DoFile("Login.lua");
        LuaManager.Instance.LuaClient.CallFunc("Login.Awake", this.gameObject);
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

}