using System.Linq;
using System.Collections.Generic;
using System;
using foundation;
using UnityEngine;
using rf;
using UnityEditor.Animations;
using UnityEditor;
using System.IO;

public class Export {

    public static ExportConfig config;

    public static Vector3[] formatVertices(Vector3[] vectors,Matrix4x4 matrix){

        Vector3[] results = new Vector3[vectors.Length];

        for (int i = 0; i < vectors.Length; i++)
        {
            Vector3 result =  matrix.MultiplyPoint(vectors[i]);
            results[i] = result;
        }

        return results;
    }


    public static Matrix4x4 TransformToMatrix4x4(Transform transform){
        Vector3 pos = transform.localPosition;
        Quaternion qua = transform.localRotation;
        Vector3 sca = transform.localScale;
        Matrix4x4 matrix = Matrix4x4.Translate(pos) * Matrix4x4.Scale(sca) * Matrix4x4.Rotate(qua);
        return matrix;
    }


    public static MeshData exportMeshData (Mesh mesh,GameObject gameObject) {


        MeshData data = new MeshData ();
        /*pos*/
        Vector3[] vectors = formatVertices(mesh.vertices,TransformToMatrix4x4(gameObject.transform));
        /*normal*/
        Vector3[] normals = mesh.normals;
        /*uv*/
        Vector2[] uvs = mesh.uv;
        Vector4[] tangnets = mesh.tangents;
        Color[] colors = mesh.colors;

        data.vertex = vector3toByte (vectors, normals, uvs, tangnets, colors);

        int numVertices = vectors.Length;

        data.numVertices = numVertices;


        int offset = 0;



        ASObject variables = new ASObject();

        if(null != vectors && numVertices == vectors.Length){
            variables.Add("pos",new Variable(3,offset));
            offset += 3;
        }

        if(null != normals && numVertices == normals.Length){
            variables.Add("normal",new Variable(3,offset));
            offset += 3;
        }

        // if(null != tangnets && 0 != tangnets.Length){
        //     variables.Add("tangnet",new Variable(4,offset));
        //     offset += 4;
        // }

        if(null != uvs && numVertices == uvs.Length){
            variables.Add("uv",new Variable(2,offset));
            offset += 2;
        }


        if(null != colors && numVertices == colors.Length){
            variables.Add("color",new Variable(4,offset));
            offset += 4;
        }

        variables.Add("data32PerVertex", new Variable(offset, offset));

        data.data32PerVertex = offset;
        data.variables = variables;



        data.index = ToUnit16(mesh.triangles);

        data.numTriangles = mesh.triangles.Length / 3;


        data.hitare = toHitArea(mesh.bounds);
        

        return data;
    }

    public static byte[] vector3toByte ( /*pos*/ Vector3[] vectors, /*normal*/ Vector3[] normals, /*uv*/ Vector2[] uvs, Vector4[] tangnets, Color[] colors) {
        //xyz 3 *2 
        int len = vectors.Length;

        Bytes bytes = new Bytes();

        // ByteArray byteArray = new ByteArray ();

        for (int i = 0; i < len; i++) {
            //pos
            writeVector (bytes, vectors, i);
            //normal
            writeVector (bytes, normals, i);
            //tangents
            writeVector (bytes, tangnets, i);
            //uv
            writeVector (bytes, uvs, i);
            //color
            writeVector (bytes, colors, i);
        }

        return bytes.ToArray ();
    }

    public static void writeVector (Bytes byteArray, Vector2[] vectors, int index) {
        if (null != vectors && 0 != vectors.Length) {
            var vector = vectors[index];
            byteArray.write (vector.x);
            byteArray.write (1 - vector.y);
        }
    }

    public static void writeVector (Bytes byteArray, Vector3[] vectors, int index) {
        if (null != vectors && 0 != vectors.Length) {
            Vector3 vector3 = vectors[index];
            byteArray.write (vector3.x);
            byteArray.write (vector3.y);
            byteArray.write (vector3.z);
        }
    }

    public static void writeVector (Bytes byteArray, Vector4[] vectors, int index) {
        if (null != vectors && 0 != vectors.Length) {
            Vector4 vector = vectors[index];
            byteArray.write (vector.x);
            byteArray.write (vector.y);
            byteArray.write (vector.z);
            byteArray.write (vector.w);
        }
    }

    public static void writeVector (Bytes byteArray, Color[] vectors, int index) {
        if (null != vectors && 0 != vectors.Length) {
            Color color = vectors[index];
            byteArray.write(color.r);
            byteArray.write(color.g);
            byteArray.write(color.b);
            byteArray.write(color.a);
        }
    }


    public static byte[] ToUnit16(int[] ints){

        Bytes bytes = new Bytes();

        for (int i = 0; i < ints.Length; i++)
        {
            bytes.write((ushort)ints[i]);
        }

        return bytes.ToArray();
    }



    public static HitArea toHitArea(Bounds bounds){
        HitArea area = new HitArea();

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        area.left = min.x;
        area.bottom = min.y;
        area.back = min.z;

        area.right = max.x;
        area.top = max.y;
        area.front = max.z;

        return area;
    }


    public static Matrix4x4 newMatrix4x4(){
        Matrix4x4 m = new Matrix4x4();
        m[0] = m[5] = m[10] = m[15] = 1.0f;
        return m;
    }


    public static UnityBounds toUntiyBounds(Bounds bounds,Matrix4x4 matrix){

        UnityBounds unityBounds = new UnityBounds();

        unityBounds.max = matrix.MultiplyPoint(bounds.max);
        unityBounds.min = matrix.MultiplyPoint(bounds.min);

        return unityBounds;
    }


    public static byte[] Matrix4x4ToBytes(Matrix4x4 m){

        Bytes bytes = new Bytes();

        for (int i = 0; i < 16; i++)
        {
            bytes.write(m[i]);
        }

        return bytes.ToArray();
    }


    public static byte[] boneWeightToBytes(BoneWeight[] boneWeights){

        Bytes bytes = new Bytes();

        for (int i = 0; i < boneWeights.Length; i++)
        {
            BoneWeight weight = boneWeights[i];

            bytes.write((float)weight.boneIndex0);
            bytes.write((float)weight.boneIndex1);
            bytes.write((float)weight.boneIndex2);
            bytes.write((float)weight.boneIndex3);

            bytes.write(weight.weight0);
            bytes.write(weight.weight1);
            bytes.write(weight.weight2);
            bytes.write(weight.weight3);
        }

        return bytes.ToArray();
    }

    public static Transform SkeletonGetRootBone(Transform bone,GameObject obj){

        while(bone.parent && bone.parent != obj.transform){
            bone = bone.parent;
        }

        return bone;
    }


    public static SkeletonData exportSkeleton(SkinnedMeshRenderer renderer,GameObject obj){

        var rootBone = SkeletonGetRootBone(renderer.rootBone,obj);
        var bones = renderer.bones;
        var mesh = renderer.sharedMesh;
        var poses = mesh.bindposes;

        SkeletonData data = new SkeletonData();

        data.root = boneToBoneData(rootBone,TransformToMatrix4x4(renderer.gameObject.transform) ,null,bones,poses);

        data.vertex = boneWeightToBytes(mesh.boneWeights);
        data.boneCount = bones.Length;
        data.numVertices = mesh.boneWeights.Length;
        data.data32PerVertex = 8;

        return data;         
    }


    public static BoneData boneToBoneData(Transform bone,Matrix4x4 inv,BoneData parent,Transform[] bones,Matrix4x4[] poses){

        // Vector3 pos = bone.localPosition;
        // Quaternion qua = bone.localRotation;
        // Vector3 sca = bone.localScale;
        // pos.Set(2,0,0);
        // sca.Set(2,0,0);
        // qua.Set(0,0,0,1);
        Matrix4x4 matrix = TransformToMatrix4x4(bone);
        // Matrix4x4.Translate(pos) * Matrix4x4.Scale(sca) * Matrix4x4.Rotate(qua);

        Matrix4x4 temp = Matrix4x4.Inverse(inv * matrix);

        BoneData data = new BoneData();
        data.name = bone.name;
        data.index = Array.IndexOf(bones,bone);
        data.matrix = Matrix4x4ToBytes(matrix);
        if(data.index > -1){
            data.inv = Matrix4x4ToBytes((inv * poses[data.index].inverse).inverse);
        }else{
            data.inv = Matrix4x4ToBytes(temp);
        }
        
        data.parent = parent;

       

        int childCount = bone.childCount;

        if(childCount > 0){
            data.children = new BoneData[childCount];
            for (int i = 0; i < childCount; i++)
            {
                BoneData child = boneToBoneData(bone.GetChild(i),inv,data,bones,poses);
                data.children[i] = child;
            }
        }else{
            data.children = new BoneData[0];
        }

        return data;
    }

    //====================================================================================================
    //
    //
    //====================================================================================================

    public static void foramtCurveData(Dictionary<string,CurveData> data,Transform bone,string path){
        // path = path + bone.name;
        data.Add(bone.name,new CurveData(bone));
        // path = path + "/";

        int childCount = bone.childCount;
        if(childCount > 0){
            for (int i = 0; i < childCount; i++)
            {
                foramtCurveData(data,bone.GetChild(i),path);
            }
        }

    }

    public static SkeletonAnimationData[] exportAnimation(SkinnedMeshRenderer renderer,GameObject gameObject){


        var root = SkeletonGetRootBone(renderer.rootBone,gameObject);
        var bones = renderer.bones;

        // renderer.sharedMesh.
        

        Dictionary<string,CurveData> boneObj = new Dictionary<string,CurveData>();
        foramtCurveData(boneObj,root,"");
        
        Animator aniamtor = gameObject.GetComponentInChildren<Animator>();
        AnimatorController controller = aniamtor.runtimeAnimatorController as AnimatorController;

        SkeletonAnimationData[] animDatas = null;
        if(controller){
            AnimationClip[] clips = controller.animationClips;
            animDatas = new SkeletonAnimationData[clips.Length];

            for (int i = 0; i < clips.Length; i++)
            {
                animDatas[i] = clipToAnimationData(clips[i],boneObj);
            }
        }

        return animDatas;
    }


    public static SkeletonAnimationData clipToAnimationData(AnimationClip clip,Dictionary<string,CurveData> boneObj){


        var frameCount = (int)Math.Floor(clip.length * clip.frameRate) + 1;
        var curveBinds = AnimationUtility.GetCurveBindings(clip);

        foreach (var item in curveBinds)
        {
            if (item.type == typeof(Transform))
            {

                var path = item.path;
                var i = path.LastIndexOf("/");
                if(i != -1){
                    path = path.Substring(i+1);
                }
                
                CurveData data = boneObj[path];

                var curve = AnimationUtility.GetEditorCurve(clip,item);
                
                switch (item.propertyName)
                {
                    case "m_LocalPosition.x":
                        data.pos_curve[0] = curve;
                        break;
                    case "m_LocalPosition.y":
                        data.pos_curve[1] = curve;
                        break;
                    case "m_LocalPosition.z":
                        data.pos_curve[2] = curve;
                        break;
                    case "m_LocalRotation.x":
                        data.qua_curve[0] = curve;
                        break;
                    case "m_LocalRotation.y":
                        data.qua_curve[1] = curve;
                        break;
                    case "m_LocalRotation.z":
                        data.qua_curve[2] = curve;
                        break;
                    case "m_LocalRotation.w":
                        data.qua_curve[3] = curve;
                        break;
                    case "m_LocalScale.x":
                        data.sca_curve[0] = curve;
                        break;
                    case "m_LocalScale.y":
                        data.sca_curve[1] = curve;
                        break;
                    case "m_LocalScale.z":
                        data.sca_curve[2] = curve;
                        break;
                }
            }
        }



        
        SkeletonAnimationData animData = new SkeletonAnimationData();
        animData.name = Export.getAnimationName(clip);
        animData.duration = clip.length;
        animData.totalFrame = frameCount;
        animData.eDuration = 1 / clip.frameRate;
        
        animData.frames = new Dictionary<string, byte[]>();


        foreach (var item in boneObj)
        {

            var curve = item.Value;
            var name = curve.bone.name;
            Bytes bytes = new Bytes();
            for (int i = 0; i < frameCount; i++)
            {
                float now = i / clip.frameRate;
                Matrix4x4 matrix = curve.toMatrix(now);
                bytes.write(Matrix4x4ToBytes(matrix));
            }
            animData.frames.Add(name,bytes.ToArray());
            bytes.Dispose();
        }

        return animData;
    }



    //===================================================================================================
    //  export material
    //===================================================================================================
    public static MaterialData ExportMaterial(Material material,UnityScene unitScene = null){
        var path=AssetDatabase.GetAssetPath(material.mainTexture);
        // var dependencies=AssetDatabase.GetDependencies(path);
        // File file = new File(path);
        // File.Copy(path,outPath+"diff.png",true);

        MaterialData data = new MaterialData();
        data.depthMask = true;
        data.passCompareMode = 0x0203;//LEQUAL
        data.srcFactor = 0x0302;//SRC_ALPHA
        data.dstFactor = 0x0303;//ONE_MINUS_SRC_ALPHA
        data.cull = 0;//NONE
        data.diffTex = new TextureData();
        data.diffTex.mipmap = true;
        data.diffTex.repeat = 0x2901;//REPEAT
        data.diffTex.mag = 0x2601;//LINEAR
        data.diffTex.mix = 0x2702;//NEAREST_MIPMAP_LINEAR

        if("" != path){
            data.diffTex.url = path.ToLower().Replace("assets/","");
            if(null != unitScene){
                // unitScene
                if(false == unitScene.materialObj.ContainsKey(path)){
                    unitScene.materialObj.Add(path,data.diffTex.url);
                }
            }
        }

        
        

        return data;

    }








    public static UnityNodeData GameObjectToUnityNodeData(GameObject obj){
        UnityNodeData data = new UnityNodeData();

        data.name = obj.name;
        Matrix4x4 matrix = TransformToMatrix4x4(obj.transform);
        data.matrix = Matrix4x4ToBytes(matrix);
        data.children = new List<UnityNodeData>();

        return data;
    }



    public static string GetAssetPath(UnityEngine.Object target){
        var path = AssetDatabase.GetAssetPath(target);
        if("" != path){
            return path;
        }

        var obj = PrefabUtility.GetCorrespondingObjectFromSource(target);
        if(null != obj){
            path = AssetDatabase.GetAssetPath(obj);
        }

        return path;
    }


    public static string getRefName(string path){
        path = path.ToLower().Replace("assets/","");
        int i = path.IndexOf(".");
        if(i != -1){
            path = path.Substring(0,i);
        }
        return path;
    }

    public static string getAnimationName(AnimationClip clip){
        var path = GetAssetPath(clip);
        var i = path.IndexOf("@");
        if(i != -1){
            i += 1;
            var j = path.IndexOf(".");
            if(j != -1){
                path = path.Substring(i,j-i);
            }else{
                path = path.Substring(i);
            }
            
        }else{
            path = clip.name;
        }

        return (path + ".kf").ToLower();
    }

    public static string getAnimationPath(AnimationClip clip){
        var path = GetAssetPath(clip);
        path = TFile.Join(TFile.PathParent(path), getAnimationName(clip));
        return path.ToLower();
    }

}