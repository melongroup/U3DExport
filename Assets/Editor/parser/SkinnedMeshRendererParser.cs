using System.Collections.Generic;
using System;
using UnityEngine;

namespace rf
{
    public class SkinnedMeshRendererParser : MeshRendererParser
    {

        protected int nameIndex = 0;
        public override object parser(GameObject _object, Component component, UnityScene unityScene){

            UnityVertexData skeletonData = null;

            SkinnedMeshRenderer renderer = component as SkinnedMeshRenderer;

            skeletonData = parserSkeleton(renderer,unityScene);

            UnityMeshData meshData = base.parser(_object,component,unityScene) as UnityMeshData;

            meshData.skeleton = skeletonData;

            return meshData;
        }


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

            // getUsedSkeleton(mesh.boneWeights);

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
    }
}