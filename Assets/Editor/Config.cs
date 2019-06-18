using System.IO;
using foundation;
using System.Text;
using UnityEngine;


using UnityEditor;
[InitializeOnLoad]
public class Setup {
    static Setup()   
    {
        // Debug.Log("up and running");    
        if(null == Export.config){
            reload();
        }
    }

    static string PATH = "config.json";

    public static void reload(){

        new TFile("aacc");

        if(File.Exists(PATH)){
            string json = File.ReadAllText(PATH, Encoding.UTF8);
            Export.config = JsonUtility.FromJson<ExportConfig>(json);
        }else{
            Export.config = new ExportConfig();
            Export.config.outdir = Application.dataPath.Replace("Assets","")+"out/";
            save();
        }
    }

    public static void save(){
        string json = JsonUtility.ToJson(Export.config);
        // Application.dataPath.Replace("Assets","")
        File.WriteAllText(PATH,json);
    }
}

public class ExportConfig{
    
    public string outdir;
    
}