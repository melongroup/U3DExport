using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace rf
{
    public static class SerializeObject
    {
        // private readonly static Int32[] LAYER = { 0x000002, 0x000004, 0x000008, 0x000010, 0x000020, 0x000040, 0x000080, 0x0000f0, 0x000100, 0x000200, 0x000400, 0x000800, 0x000f00 };
        private readonly static Dictionary<string, IComponentParser> componentParsers = new Dictionary<string, IComponentParser>();

        public static void Initialize()
        {
            //初始化组件管理器
            componentParsers.Clear();
            // RegComponentParser(new AniPlayerParser(),               typeof(FB.PosePlus.AniPlayer),              "egret3d.Animation");
            RegComponentParser(new AnimatorParser(), typeof(UnityEngine.Animator));
            // RegComponentParser(new AnimationParser(), typeof(UnityEngine.Animation), "egret3d.Animation");
            // RegComponentParser(new BoxColliderParser(), typeof(UnityEngine.BoxCollider), "egret3d.BoxCollider");
            // RegComponentParser(new SphereColliderParser(), typeof(UnityEngine.SphereCollider), "egret3d.SphereCollider");
            RegComponentParser(new CameraParser(), typeof(Camera)); ;
            // RegComponentParser(new MeshFilterParser(), typeof(UnityEngine.MeshFilter), "egret3d.MeshFilter");
            RegComponentParser(new MeshRendererParser(), typeof(MeshRenderer)); ;
            // RegComponentParser(new ParticleSystemParser(), typeof(UnityEngine.ParticleSystem), "egret3d.particle.ParticleComponent");
            // RegComponentParser(new ParticleSystemRendererParser(), typeof(UnityEngine.ParticleSystemRenderer), "egret3d.particle.ParticleRenderer");
            RegComponentParser(new SkinnedMeshRendererParser(), typeof(SkinnedMeshRenderer));
            // RegComponentParser(new TransformParser(), typeof(UnityEngine.Transform), "egret3d.Transform");
            RegComponentParser(new LightParser(), typeof(Light));
            // RegComponentParser(new SpotLightParser(), typeof(UnityEngine.Light), "egret3d.SpotLight");
        }
        /**
         * 注册可序列化组件。
         * @param parser 序列化组件实例。
         * @param compType 对应Unity的类型。
         * @param className 对应Egret3d的类名(例如：egret3d.Animation)。
         */
        public static void RegComponentParser(IComponentParser parser, System.Type compType)
        {
            parser.compType = compType;
            // parser.className = className;
            if (!componentParsers.ContainsKey(compType.Name))
            {
                componentParsers[compType.Name] = parser;
            }
        }

        public static object ExecComponentParser(GameObject obj, Component component,UnityScene unityScene){
            if(componentParsers.Count == 0){
                Initialize();
            }
            string className = component.GetType().Name;
            if(componentParsers.ContainsKey(className)){
                return componentParsers[className].parser(obj,component,unityScene);
            }

            return null;
        }

        /**
        *是否可以序列化该组件
        */
        public static bool IsComponentSupport(string className)
        {
            return componentParsers.ContainsKey(className);
        }


        public static UnityScene ExportScene(Scene scene){
            UnityScene unitScene = new UnityScene();
            var list = scene.GetRootGameObjects();
            foreach (var obj in list)
            {
                UnityPerfabData data = Serialize(obj,unitScene);
                if(null != data){
                    unitScene.perfabDatas.Add(data);
                }
            }
            return unitScene;
        }



        public static void ExportGameObject(GameObject obj,UnityScene unitScene = null){

            if(null == unitScene){
                unitScene = new UnityScene();
            }

            Serialize(obj,unitScene);
            ExportUnitScene(unitScene);

        }



        public static UnityPerfabData Serialize(GameObject obj,UnityScene unitScene = null)
        {
            if(null == unitScene){
                unitScene = new UnityScene();
            }

            unitScene.current = null;

            if(!obj.activeInHierarchy){
                //对象未激活 不参与导出
                return null;
            }

            UnityPerfabData data = null;
            string refName = null;

            if (obj.GetComponent<RectTransform>() != null)
            {
                return null;
            }

            Debug.Log("导出对象:" + obj.name);

            //判断是否是Camera
            var camera = obj.GetComponent<Camera>();
            if(null != camera){
                ExecComponentParser(obj,camera,unitScene);
                return null;
            }

            var light = obj.GetComponent<Light>();
            if(null != light){
                ExecComponentParser(obj,light,unitScene);
                return null;
            }

            //prefab
            var prefabType=PrefabUtility.GetPrefabAssetType(obj);
            if(prefabType != PrefabAssetType.NotAPrefab){
                var parentObj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                refName = AssetDatabase.GetAssetPath(parentObj);
                if(unitScene.refPerfabDatas.ContainsKey(refName)){
                    return unitScene.refPerfabDatas[refName];
                }
            }



            var animator = obj.GetComponent<Animator>();
            if(null != animator){
                data = ExecComponentParser(obj,animator,unitScene) as UnityPerfabData;
                if(null != data){
                    return data;
                }
            }


            var meshRenderer = obj.GetComponent<MeshRenderer>();    
            if(null != meshRenderer){
                data = ExecComponentParser(obj,animator,unitScene) as UnityPerfabData;
            }else{
                var skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
                if(null != skinnedMeshRenderer){
                    data = ExecComponentParser(obj,animator,unitScene) as UnityPerfabData;
                }else{
                    data = new UnityPerfabData(obj);
                }
            }

            // var particle = obj.GetComponent<ParticleSystem>();
            //todo particle


            UnityRefPerfabData tempData = unitScene.current;

            Transform transform = obj.transform;

            int count = transform.childCount;

            for (int i = 0; i < count; i++)
            {
                var temp = Serialize(transform.GetChild(i).gameObject,unitScene);
                if(null != temp){
                    data.childrens.Add(temp);
                }
            }

            unitScene.current = tempData;


            return data;

            // MyJson_Object item = new MyJson_Object();
            // item.SetUUID(obj.GetInstanceID().ToString());
            // item.SetUnityID(obj.GetInstanceID());
            // item.SetClass("paper.GameObject");
            // item.SetString("name", obj.name);
            // item.SetString("tag", obj.tag);
            // var layerMask = 1 << obj.layer;
            // item.SetInt("layer", layerMask);
            // // item.SetInt("layer", LAYER[obj.layer >= LAYER.Length ? 0 : obj.layer]);;
            // item.SetBool("isStatic", obj.isStatic);

            // var componentsItem = new MyJson_Array();
            // item["components"] = componentsItem;
            // ResourceManager.instance.AddObjectJson(item);

            // var components = obj.GetComponents<Component>();

            // var index = 0;//TODO
            // foreach (var comp in components)
            // {
            //     if (comp is Animator)
            //     {
            //         components[index] = components[0];
            //         components[0] = comp;
            //     }

            //     index++;
            // }

            // //遍历填充组件
            // foreach (var comp in components)
            // {
            //     if (comp == null)
            //     {
            //         MyLog.LogWarning("空的组件");
            //         continue;
            //     }
            //     string compClass = comp.GetType().Name;
            //     MyLog.Log("组件:" + compClass);
            //     if (!ExportToolsSetting.instance.exportUnactivatedComp)
            //     {
            //         //利用反射查看组件是否激活，某些组件的enabled不再继承链上，只能用反射，比如BoxCollider
            //         var property = comp.GetType().GetProperty("enabled");
            //         if (property != null && !((bool)property.GetValue(comp, null)))
            //         {
            //             MyLog.Log(obj.name + "组件未激活");
            //             continue;
            //         }
            //     }

            //     if (!SerializeObject.IsComponentSupport(compClass))
            //     {
            //         MyLog.LogWarning("不支持的组件： " + compClass);
            //         continue;
            //     }
            //     if (SerializeObject.SerializedComponent(obj, compClass, comp))
            //     {
            //         MyLog.Log("--导出组件:" + compClass);
            //         componentsItem.AddHashCode(comp);
            //     }
            //     else
            //     {
            //         MyLog.LogWarning("组件： " + compClass + " 导出失败");
            //     }
            // }
            // //遍历子对象
            // if (obj.transform.childCount > 0)
            // {
            //     for (int i = 0; i < obj.transform.childCount; i++)
            //     {
            //         var child = obj.transform.GetChild(i).gameObject;
            //         Serialize(child);
            //     }
            // }
        }


        public static void ExportUnitScene(UnityScene unitScene){

            //write avatar
            //step1 write mesh
            foreach (var item in unitScene.avatarObj)
            {
                var avatar = item.Value;
                TFile file = new TFile(Export.config.outdir).ResolvePath(avatar.refName+".km");
                file.WriteAMF(avatar);
            }

            //write animation

            foreach (var item in unitScene.animationDataObj){
                var path = item.Key.Replace("assets/","");
                var clip = item.Value;
                TFile file = new TFile(Export.config.outdir).ResolvePath(path);
                file.WriteAMF(clip);
            }


            foreach (var item in unitScene.materialObj){
                string key = item.Key;

                string value = TFile.Join(Export.config.outdir,item.Value);

                var file = TFile.AppliationRoot.ResolvePath(key);
                file.CopyTo(value);

            }


        }
       
    }
}
