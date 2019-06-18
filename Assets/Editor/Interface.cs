using System;
using System.Collections.Generic;
using foundation;
using UnityEngine;





public class Variable{
    public int size;
    public int offset;

    public Variable(int size,int offset){
        this.size = size;
        this.offset = offset;
    }
}


public class HitArea{
    public float left = 0.0f;
    public float right = 0.0f;
    public float top = 0.0f;
    public float bottom = 0.0f;
    public float front = 0.0f;
    public float back = 0.0f;
}


public class UnityBounds{
    public Vector3 max;
    public Vector3 min;
    public Vector3 center;
}

public class MeshData{
    public byte[] vertex;
    public byte[] index;
    public ASObject variables;
    public int numVertices;
    public int numTriangles;
    public int data32PerVertex;
    public HitArea hitare;
    public float nameLabelY = 0.0f;
}

public class TextureData{
    public string key;
    public string url;
    public Boolean mipmap;
    public int mag;
    public int mix;
    public int repeat;
}

public class MaterialData{
    public Boolean depthMask;
    public int passCompareMode;
    public int srcFactor;
    public int dstFactor;
    public int cull;
    public float alphaTest = 0.0f; //0表示不剔除
    public TextureData diffTex;
    public TextureData specularTex;
    public TextureData normalTex;
    public TextureData emissiveTex;
}


//=====================================================================
//Bone
//=====================================================================

public class BoneData{
    public byte[] inv;
    public byte[] matrix;
    public string name;
    public int index;
    public BoneData parent;
    public BoneData[] children;
}


public class SkeletonData{
    public byte[] vertex;
    public BoneData root;
    public int data32PerVertex;
    public int numVertices;
    public int boneCount;
}


public class SkeletonAnimationData{
        public string name;
        public float duration;
        public float eDuration;
        public int totalFrame;
        public Dictionary<string,Byte[]> frames;
    }



//==================================================================

public class SkeletonMeshData{
    public MeshData mesh;
    public MaterialData material;
    public SkeletonData skeletonData;
    public String[] anims;
    public Boolean shadowCast;
    public Boolean sun;
}



//========================================================================================
public class CurveData{
    public Transform bone;
    public int index;
    public AnimationCurve[] pos_curve;
    public AnimationCurve[] qua_curve;
    public AnimationCurve[] sca_curve;
    public CurveData(Transform bone){
        this.bone = bone;
        this.pos_curve = new AnimationCurve[3];
        this.qua_curve = new AnimationCurve[4];
        this.sca_curve = new AnimationCurve[3];
    }

    public Matrix4x4 toMatrix(float now){

        Vector3 pos = new Vector3(bone.localPosition.x,bone.localPosition.y,bone.localPosition.z);
        Quaternion qua = new Quaternion(bone.localRotation.x,bone.localRotation.y,bone.localRotation.z,bone.localRotation.w);
        Vector3 sca = new Vector3(bone.localScale.x,bone.localScale.y,bone.localScale.z);

        for (int i = 0; i < 3; i++)
        {
            var curve = pos_curve[i];
            if(null != curve){
                pos[i] = curve.Evaluate(now);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            var curve = qua_curve[i];
            if(null != curve){
                qua[i] = curve.Evaluate(now);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            var curve = sca_curve[i];
            if(null != curve){
                sca[i] = curve.Evaluate(now);
            }
        }

        return Matrix4x4.Translate(pos) * Matrix4x4.Scale(sca) * Matrix4x4.Rotate(qua);
    }
}

public class PrefabElement{
    public string refName;
    public string name;
    /**
    1 : mesh
    2 : animation
    */
    public int type;
    public SkeletonMeshData meshData;
}


public class PrefabData{
    public Byte[] matrix;
    public string name;
    public string refName;
    /**
    1 : mesh
    2 : animation
    */
    public int type; //类型
    public PrefabData[] childrens;
    // public PrefabData parent;
    public PrefabData(GameObject gameObject = null){
        if(gameObject){
            Matrix4x4 matrix4x4 = Export.TransformToMatrix4x4(gameObject.transform);
            this.matrix = Export.Matrix4x4ToBytes(matrix4x4);
            this.name = gameObject.name;
        }else{
            this.matrix = Export.Matrix4x4ToBytes(new Matrix4x4());
        }
    }
}

public class SceneData:PrefabData{
    public CameraData camera;
    public DirectionalLight light;
}

public class CameraData{
    public Byte[] matrix;
}

public class DirectionalLight{
    public Byte[] matrix;
    public Byte[] color;
}