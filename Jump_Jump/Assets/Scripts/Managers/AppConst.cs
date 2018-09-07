using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AppConst
{
    public const bool DebugMode = true;                       

    public const bool LuaByteMode = true;                       //Lua字节码模式-默认关闭 

    public const string AppName = "Jump_Jump";               //应用程序名称
    public const string LuaTempDir = "MyLua/";                    //临时目录
    public const string ExtName = ".unity3d";                   //素材扩展名
    public const string AssetDir = "StreamingAssets";           //素材目录 
    public const string WebUrl = "";      //测试更新地址


    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath;
        }
    }
}