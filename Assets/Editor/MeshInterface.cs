using System.Collections.Generic;
using UnityEngine;

public class UnityVertexData{
    public byte[] vertex;
    public Dictionary<string,Variable> variables;
    public int data32PerVertex;
}


public class UnitySkeletonData : UnityVertexData{
    
}

public class UnityMeshData{
    public string node;
    public MaterialData material;
    public UnityVertexData vertex;
    public UnityVertexData skeleton;
    public UnityBounds bounds;
    public byte[] index;
    public int numTriangles;
    public int numVertices;
}


public class UnityNodeData{
    public byte[] inv;
    public byte[] matrix;
    public string name;
    public int index = -1;
    public UnityNodeData parent;
    public List<UnityNodeData> children;
}


public class UnityAvatarData{
    public string refName;
    public UnityNodeData root;
    public HitArea hitare;
    public Dictionary<string,List<UnityMeshData>> mesh;
    public UnityAnimator animator;
    public UnityAvatarData(){
        mesh = new Dictionary<string, List<UnityMeshData>>();
    }
}

public class UnityAnimator{
    public Dictionary<string,string> animObjs;

    public UnityAnimator(){
        animObjs = new Dictionary<string, string>();
    }
}


public class UnityPerfabData{
    public string name;
    public byte[] matrix;
    public List<UnityPerfabData> childrens;

    public UnityPerfabData(GameObject gameObject = null){
        childrens = new List<UnityPerfabData>();
        
        if(gameObject){
            Matrix4x4 matrix4x4 = Export.TransformToMatrix4x4(gameObject.transform);
            this.matrix = Export.Matrix4x4ToBytes(matrix4x4);
            this.name = gameObject.name;
        }else{
            this.matrix = Export.Matrix4x4ToBytes(new Matrix4x4());
        }
    }

}

public class UnityRefPerfabData:UnityPerfabData{
    /*
        1 AvatarRender
        2 Particle
     */
    public int type;
    public string refName;
}

public class UnityAvatarRenderer:UnityRefPerfabData{
    public string avatar;
    public UnityAnimator animator;
    public UnityAvatarRenderer(){
        this.type = 1;
    }
}


public class UnityScene{
    public CameraData camera;
    public DirectionalLight light;
    public Dictionary<string,UnityRefPerfabData> refPerfabDatas;
    public Dictionary<string,UnityMeshData> meshDataObj;
    public Dictionary<string,UnityAvatarData> avatarObj;
    public Dictionary<string,SkeletonAnimationData> animationDataObj;
    public Dictionary<string,UnityAnimator> animatorObj;
    public Dictionary<string,string> materialObj;
    public List<UnityPerfabData> perfabDatas;
    public UnityRefPerfabData current;
    public Renderer renderer;
    public string outpath;
    public UnityScene(){
        refPerfabDatas = new Dictionary<string, UnityRefPerfabData>();
        meshDataObj = new Dictionary<string, UnityMeshData>();
        avatarObj = new Dictionary<string, UnityAvatarData>();
        animationDataObj = new Dictionary<string, SkeletonAnimationData>();
        animatorObj = new Dictionary<string, UnityAnimator>();
        perfabDatas = new List<UnityPerfabData>();
        materialObj = new Dictionary<string, string>();
    }

    public UnityAvatarRenderer getAvatarRenderer(){
        if(null == current){
            current = new UnityAvatarRenderer();
        }

        if(null == current as UnityAvatarRenderer){
            //报错
            Debug.Log("error!");
        }

        return current as UnityAvatarRenderer;
    }


    public UnityAvatarData getAvatarData(UnityScene unityScene,string refName = null){
        UnityAvatarRenderer renderer = getAvatarRenderer();
        UnityAvatarData avatar = null;

        if(null == refName){
            refName = renderer.avatar;
        }

        if(null == refName){
            return null;
        }

        if(unityScene.avatarObj.ContainsKey(refName)){
            avatar = unityScene.avatarObj[refName];
        }else{
            avatar = new UnityAvatarData();
            unityScene.avatarObj.Add(refName,avatar);
        }

        renderer.avatar = refName;

        return avatar;
        
    }

}