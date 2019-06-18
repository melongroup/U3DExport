using System.Collections.Generic;
using System.IO;
using UnityEngine;
using foundation;
public class TFile{


    public static TFile AppliationAssets = new TFile(Application.dataPath);

    public static TFile AppliationRoot = AppliationAssets.Parent;

    public static string FormatPath(string path){
        path = path.Replace("\\","/");
        if(path.IndexOf(".") == -1 && path.LastIndexOf("/") != path.Length-1){
            path += "/";
        }
        return path;
    }

    public static string Join(string prefix,string path){
        if(path.IndexOf("/") == 0){
            path = path.Substring(1);
        }
        return FormatPath(prefix) + FormatPath(path);
    }


    public static string PathName(string path){
        var i = path.LastIndexOf("/",path.Length - 2);
        if(i == -1){
            return path;
        }
        return path.Substring(i+1);
    }

    public static string PathExtName(string path){
        var i = path.LastIndexOf(".");
        if(i == -1){
            return "";
        }
        return path.Substring(i+1);
    }

    public static string PathParent(string path){
        path = FormatPath(path);
        var i = path.LastIndexOf("/",path.Length - 2);
        if(i == -1){
            return "";
        }
        return FormatPath(path.Substring(0,i));
    }

    public string nativePath;

    public string name;

    public string extName;

    public TFile(string path){
        this.nativePath = FormatPath(path);
        this.name = PathName(path);
        this.extName = PathExtName(path);
    }

    public TFile ResolvePath(string path){
        return new TFile(Join(nativePath,path));
    }   

    public bool isFile{
        get{
            if(File.Exists(nativePath)){
                return true;
            }
            return false;
        }
    }

    public bool Exists{
        get{
            return File.Exists(nativePath) || Directory.Exists(nativePath);
        }
    }

    public TFile Parent{
        get{
            string path = PathParent(nativePath);
            if("" == path){
                return null;
            }
            return new TFile(path);
        }
    }

    public void MakeDir(){
        if(false == Exists){
            Parent.MakeDir();
            Directory.CreateDirectory(nativePath);
        }
    }

    public void CopyTo(string to,bool overwrite = true){
        CopyTo(new TFile(to),overwrite);
    }

    public void CopyTo(TFile to,bool overwrite = true){
        to.Parent.MakeDir();
        File.Copy(nativePath,to.nativePath,overwrite);
    }

    public void MoveTo(string to){
        File.Move(nativePath,to);
    }

    public void MoveTo(TFile to){
        MoveTo(to.nativePath);
    }


    public void Write(byte[] bytes){
        Parent.MakeDir();
        File.WriteAllBytes(nativePath,bytes);
    }

    public void Write(string value){
        Parent.MakeDir();
        File.WriteAllText(nativePath,value);
    }

    public void WriteAMF(object value){
        ByteArray byteArray=new ByteArray();
        byteArray.WriteObject(value);
        byteArray.Compress();
        Write(byteArray.ToArray());
    }

    public void WriteJson(Object value){
        string json = JsonUtility.ToJson(value);
        Write(json);
    }

    public byte[] Read(){
        if(Exists){
            return File.ReadAllBytes(nativePath);
        }
        return new byte[0];
    }

    public string ReadUTF8(){
        if(Exists){
            return File.ReadAllText(nativePath);
        }
        return "";
    }

    public T ReadJson<T>(){
        string json = ReadUTF8();
        return JsonUtility.FromJson<T>(json);
    }
}