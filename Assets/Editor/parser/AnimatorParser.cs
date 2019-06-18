using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace rf
{
    public class AnimatorParser : ComponentParser
    {
        public override object parser(GameObject _object, Component component, UnityScene unityScene){
            Animator animator = component as Animator;
            string path = AssetDatabase.GetAssetPath(animator.avatar);
            if( "" == path ){
                return null;
            }

            UnityAvatarData avataData = null;

            UnityAvatarRenderer avatar = unityScene.getAvatarRenderer();

            if(true == unityScene.avatarObj.ContainsKey(path)){
                avataData = unityScene.avatarObj[path];
            }else{
                avataData = unityScene.getAvatarData(unityScene,path);
                //step1 : parser Nodes
                avataData.root = toNodes(_object,unityScene,path);
                //step2 : parser meshs
                toAvatar(_object,unityScene);

                avataData.refName = getRefName(path);
                
                unityScene.avatarObj[path] = avataData;
            }

            avatar.avatar = avataData.refName;

            //step3 : parser animation
            if(animator.runtimeAnimatorController){
                avataData.animator = avatar.animator = toAnimator(_object,animator.runtimeAnimatorController as AnimatorController,unityScene);
            }

            

            return avatar;
        }


        protected UnityNodeData toNodes(GameObject obj,UnityScene unityScene,string modelPath){

            UnityNodeData nodeData = Export.GameObjectToUnityNodeData(obj);

            Transform transform = obj.transform;

            for (int i = 0; i < transform.childCount; i++)
            {   
                var child = transform.GetChild(i);
                var gameObject = child.gameObject;


                // var path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject));

                if(null == gameObject.GetComponent<Animation>()){
                    var childNodeData = toNodes(child.gameObject,unityScene,modelPath);
                    if(null != childNodeData){
                        nodeData.children.Add(childNodeData);
                        childNodeData.parent = nodeData;
                    }
                }else{
                    //todo Child Mesh
                }
            }
            

            return nodeData;
        }

        protected UnityAvatarData toAvatar(GameObject gameObject,UnityScene unityScene){

            var avatarData = unityScene.getAvatarData(unityScene);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            List<UnityMeshData> list = null;

            foreach (var renderer in renderers)
            {
                var meshdata = toMeshData(renderer,unityScene);
                if(null != meshdata){
                    if(false == avatarData.mesh.ContainsKey(meshdata.node)){
                         list = new List<UnityMeshData>();
                         avatarData.mesh.Add(meshdata.node,list);
                    }else{
                        list = avatarData.mesh[meshdata.node];
                    }
                    list.Add(meshdata);
                }
            }

            return avatarData;
        }


        protected UnityMeshData toMeshData(Renderer renderer,UnityScene unityScene){

            Mesh mesh = null;
            UnityVertexData skeletonData = null;

            if(renderer is MeshRenderer){
                mesh = renderer.GetComponent<MeshFilter>().mesh;
            }else if(renderer is SkinnedMeshRenderer){
                mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
                skeletonData = parserSkeleton(renderer as SkinnedMeshRenderer,unityScene);
            }

            if(null == mesh){
                return null;
            }

            UnityMeshData meshData = new UnityMeshData();
            UnityVertexData vertexData = new UnityVertexData();
            meshData.vertex = vertexData;


            /*pos*/
            Vector3[] vectors = mesh.vertices;
            /*normal*/
            Vector3[] normals = mesh.normals;
            /*uv*/
            Vector2[] uvs = mesh.uv;
            Vector4[] tangnets = null;//mesh.tangents;
            Color[] colors = mesh.colors;

            
            
            int numVertices = vectors.Length;
            meshData.numVertices = numVertices;

            int offset = 0;
            Dictionary<string,Variable> variables = new Dictionary<string,Variable>();

            if(null != vectors && numVertices == vectors.Length){
                variables.Add("pos",new Variable(3,offset));
                offset += 3;
            }

            if(null != normals && numVertices == normals.Length){
                variables.Add("normal",new Variable(3,offset));
                offset += 3;
            }

            if(null != tangnets && 0 != tangnets.Length){
                variables.Add("tangnet",new Variable(4,offset));
                offset += 4;
            }

            if(null != uvs && numVertices == uvs.Length){
                variables.Add("uv",new Variable(2,offset));
                offset += 2;
            }


            if(null != colors && numVertices == colors.Length){
                variables.Add("color",new Variable(4,offset));
                offset += 4;
            }

            variables.Add("data32PerVertex", new Variable(offset, offset));


            vertexData.vertex = Export.vector3toByte (vectors, normals, uvs, tangnets, colors);

            vertexData.data32PerVertex = offset;
            vertexData.variables = variables;



            meshData.index = Export.ToUnit16(mesh.triangles);

            meshData.numTriangles = mesh.triangles.Length / 3;

            meshData.bounds = Export.toUntiyBounds(mesh.bounds,Export.newMatrix4x4());

            meshData.skeleton = skeletonData;


            meshData.material = Export.ExportMaterial(renderer.material,unityScene);

            meshData.node = renderer.gameObject.name;

            if( "" == meshData.node ){
                Debug.LogError("error node name");
            }



            return meshData;
        }


        protected int nameIndex = 0;


        protected Dictionary<string,UnityNodeData> skeletonRoot2Dict(UnityNodeData root,Dictionary<string,UnityNodeData> skeletonDict = null){
            if(null == skeletonDict){
                skeletonDict = new Dictionary<string, UnityNodeData>();
            }

            string name = root.name;
            if(null == name || name == ""){
                name = "Auto_Instance_"+nameIndex++;
            }

            skeletonDict[name] = root;

            foreach (var item in root.children)
            {
                skeletonRoot2Dict(item,skeletonDict);
            }

            return skeletonDict;
        }




        protected UnityVertexData parserSkeleton(SkinnedMeshRenderer renderer,UnityScene scene){
            var bones = renderer.bones;
            var mesh = renderer.sharedMesh;
            var poses = mesh.bindposes;

            nameIndex = 0;

            UnityVertexData data = new UnityVertexData();
            data.vertex = Export.boneWeightToBytes(mesh.boneWeights);
            data.data32PerVertex = 8;

            var dict = skeletonRoot2Dict(scene.getAvatarData(scene).root);

            for (int i = 0; i < bones.Length; i++)
            {
                var bone = bones[i];
                if(dict.ContainsKey(bone.name)){
                    var node = dict[bone.name];
                    node.inv = Export.Matrix4x4ToBytes(poses[i]);
                    node.index = i;
                }
            }

            getUsedSkeleton(mesh.boneWeights);

            return data;
        }



        protected Dictionary<float,float> getUsedSkeleton(BoneWeight[] boneWeights){

            Dictionary<float,float> usedSkeleton = new Dictionary<float, float>();

            float j = 0.0f;

            for (int i = 0; i < boneWeights.Length; i++){
                
                BoneWeight weight = boneWeights[i];

                if(usedSkeleton.ContainsKey(weight.boneIndex0) == false){
                    usedSkeleton.Add(weight.boneIndex0,j++);
                }

                if(usedSkeleton.ContainsKey(weight.boneIndex1) == false){
                    usedSkeleton.Add(weight.boneIndex1,j++);
                }

                if(usedSkeleton.ContainsKey(weight.boneIndex2) == false){
                    usedSkeleton.Add(weight.boneIndex2,j++);
                }

                if(usedSkeleton.ContainsKey(weight.boneIndex3) == false){
                    usedSkeleton.Add(weight.boneIndex3,j++);
                }
            }

            return usedSkeleton;

        }






        protected UnityAnimator toAnimator(GameObject obj,AnimatorController controller,UnityScene unityScene){


            var path = AssetDatabase.GetAssetPath(controller);

            if( "" == path){
                return null;   
            }


            if(true == unityScene.animatorObj.ContainsKey(path)){
                return unityScene.animatorObj[path];
            }

            UnityAnimator animator = new UnityAnimator();
            unityScene.animatorObj.Add(path,animator);

            Dictionary<string,CurveData> boneObj = new Dictionary<string,CurveData>();
            Export.foramtCurveData(boneObj,obj.transform,"");

            AnimationClip[] clips = controller.animationClips;
            int length = clips.Length;

            for (int i = 0; i < length; i++)
            {
                var clip = clips[i];
                // animDatas[i] = Export.clipToAnimationData(clips[i],boneObj);
                var refName = Export.getAnimationPath(clip); 
                var name = Export.getAnimationName(clip);

                if(false == unityScene.animationDataObj.ContainsKey(refName)){
                    var data = Export.clipToAnimationData(clip,boneObj);
                    unityScene.animationDataObj.Add(refName,data);
                }

                animator.animObjs.Add(name,refName.Replace("assets/",""));
            }



            





            return animator;
        }

        




    }
}