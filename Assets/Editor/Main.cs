using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using foundation;
using UnityEditor.Animations;
using rf;

public class ExportWindow : EditorWindow
{
    [MenuItem("Tools/export",false,1)]
    new static void Show()
    {
        var window = GetWindow<ExportWindow>();
        window.minSize = new Vector2(500, 500);
        SerializeObject.Initialize();
    }


    public static Dictionary<string,PrefabElement> exportedObj = null;

    public void OnGUI()
    {
        //  ByteArray byteArray=new ByteArray();
        // byteArray.WriteFloat()

        // float f=1.236f;
        // var bytes=System.BitConverter.GetBytes(f);		
        // byteArray.WriteBytes(bytes,0,bytes.Length);
        // byteArray.WriteObject();
        // byteArray.Compress()
        // foundation.AmfHelper.saveByteArray(byteArray,"");
        EditorGUILayout.BeginVertical();
        // var scene = SceneManager.GetActiveScene();
        // var list = scene.GetRootGameObjects();
        if (GUILayout.Button("export Scene" ,EditorStyles.miniButton,GUILayout.MinHeight(40)))
        {
            //ShowNotification(new GUIContent("hello world!"));
            exportedObj = new Dictionary<string, PrefabElement>();
            exportScene();
        }
        // foreach (var item in list)
        // {
        //     // EditorGUILayout.BeginHorizontal();
        //     if(item.GetComponentInChildren<MeshRenderer>() || item.GetComponentInChildren<SkinnedMeshRenderer>()){
        //         if (GUILayout.Button("export " + item.name,EditorStyles.miniButton,GUILayout.MinHeight(40)))
        //         {
        //             //ShowNotification(new GUIContent("hello world!"));
        //             exportedObj = new Dictionary<string, PrefabElement>();
        //             exportMesh(item);
        //         }
        //     }
        // }
        EditorGUILayout.EndVertical();
        
        


        // if (GUILayout.Button("export",EditorStyles.miniButton,GUILayout.MaxWidth(200),GUILayout.MinHeight(40)))
        // {
            // initSceneMesh();
        // }
        //    if (GUILayout.Button("hello", EditorStyles.miniButtonMid))
        //    {
        //        ShowNotification(new GUIContent("hello world!"));
        //    }
        //    if (GUILayout.Button("hello", EditorStyles.miniButtonRight))
        //    {
        //        ShowNotification(new GUIContent("hello world!"));
        //    }
        // EditorGUILayout.EndHorizontal();
    }



    public PrefabData createPrefabData(GameObject gameObject){
        // Transform transform = gameObject.transform;
        // PrefabData data = new PrefabData(gameObject);
        return null;
    }

    
    [MenuItem("Tools/quickExportScene %q",false,13)]
    static void exportScene(){
        var scene = SceneManager.GetActiveScene();
        if(null != scene){
            SerializeObject.ExportScene(scene);
        }
        

        /* 


        var list = scene.GetRootGameObjects();

        PrefabData prefabData = new PrefabData(null);
        prefabData.matrix = Export.Matrix4x4ToBytes(new Matrix4x4());
        prefabData.name = "scene";
        List<PrefabData> prefabs = new List<PrefabData>();

        foreach (var item in list)
        {

            if(item.tag == "MainCamera"){
                //todo Camera Setting
            }



            var prefabType=PrefabUtility.GetPrefabAssetType(item);
            if(prefabType!=PrefabAssetType.NotAPrefab){

                

                PrefabData data = createPrefabData(item);
                if(null != data){
                    prefabs.Add(data);
                }
                



                var len= item.transform.childCount;
                Debug.Log("item length:" + len);
                
// for (int i = 0; i < len; i++)
// {
//    var child= item.transform.GetChild(i);

   
//             if(prefabType==PrefabAssetType.NotAPrefab){

//             }else{

// PrefabUtility.GetPrefabParent()

//             }
//     child.name;
//     child.refName;
//     child.transform;

// }
            }


            // exportMesh(item);

            // var render = item.GetComponentInChildren<MeshRenderer>();
            // if (render)
            // {
            //     EditorGUILayout.ObjectField(render.gameObject, typeof(MeshRenderer), false);
            //     mesh = render.GetComponent<MeshFilter>().mesh;
            // }
            // else
            // {
            //     var skinRender = item.GetComponentInChildren<SkinnedMeshRenderer>();

            //     if (skinRender)
            //     {
            //         EditorGUILayout.ObjectField(skinRender.gameObject, typeof(SkinnedMeshRenderer), false);
            //         mesh=skinRender.sharedMesh;
            //             //skinRender.bones;
            //     }
            // }


            // if(mesh){
            //     exportMesh(mesh);
            // }

        }

        prefabData.childrens = prefabs.ToArray();

        */
    }



    public string Path2ResPath(string path){
        path = path.Replace("Assets/","");

        // path.Replace

        return path;
    }


    public PrefabElement exportMesh(GameObject gameObject){

        Mesh mesh=null;
        Material material=null;
        MeshData meshData = null;
        SkeletonData skeletonData = null;
        SkeletonAnimationData[] animDatas = null;
        GameObject skinObject = null;

        PrefabElement element = null;
        string path = null;

       

        var render = gameObject.GetComponentInChildren<MeshRenderer>();
        if (render)
        {
            //EditorGUILayout.ObjectField(render.gameObject, typeof(MeshRenderer), false);
            mesh = render.GetComponent<MeshFilter>().mesh;
            material = render.material;
            skinObject = render.gameObject;

            path=AssetDatabase.GetAssetPath(mesh);
            if(exportedObj.ContainsKey(path)){
                return exportedObj[path];
            }



            
        }else{
            var skinRender = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinRender){
                //EditorGUILayout.ObjectField(skinRender.gameObject, typeof(SkinnedMeshRenderer), false);
                mesh = skinRender.sharedMesh;
                material = skinRender.sharedMaterial;
                skinObject = skinRender.gameObject;

                path=AssetDatabase.GetAssetPath(mesh);

                if(exportedObj.ContainsKey(path)){
                    return exportedObj[path];
                }

                skeletonData = Export.exportSkeleton(skinRender,gameObject);

                var animator = gameObject.GetComponentInChildren<Animator>();
                var avatar = animator.avatar;
                var bones = skinRender.bones;
                var rootBone = skinRender.rootBone;

                animDatas = Export.exportAnimation(skinRender, gameObject);

                

                // var controller = animator.runtimeAnimatorController as AnimatorController;
                // var layer = controller.layers[0] as AnimatorControllerLayer;
                // var clips=controller.animationClips;
            }

        }

        if(mesh){

            Debug.Log("parse " + gameObject.name);


            meshData = Export.exportMeshData(mesh,skinObject);

            ByteArray byteArray = null;

            SkeletonMeshData data = new SkeletonMeshData();
            data.mesh = meshData;
            data.skeletonData = skeletonData;

            string root = "E:/project/h5games/data/";


            string refName = path.Replace("Assets/","");
            int dotIndex = refName.IndexOf(".");
            refName = refName.Substring(0,dotIndex) + "/";

            string outpath = root + refName;

           
            if(null != animDatas){
                data.anims = new string[animDatas.Length];
                for (int i = 0; i < animDatas.Length; i++)
                {
                    SkeletonAnimationData animData = animDatas[i];
                    byteArray=new ByteArray();
                    byteArray.WriteObject(animData);
                    byteArray.Compress();
                    AmfHelper.saveByteArray(byteArray,root+animData.name);
                    data.anims[i] = animData.name;
                    
                }
            }

            if(null != material){
                data.material = Export.ExportMaterial(material);
                string diffpath = data.material.diffTex.url;
                File.Copy(diffpath,outpath+"diff.png",true);
            }


            byteArray=new ByteArray();
            byteArray.WriteObject(data);
            byteArray.Compress();
            AmfHelper.saveByteArray(byteArray,outpath + "mesh.km");
            
            //element
            element = new PrefabElement();
            element.name = path;
            element.refName = refName;
            element.meshData = data;
            exportedObj.Add(path,element);

        }


        // return Path.GetFileName(path).Replace(Path.GetExtension(path),"");

        return element;


    }

}
