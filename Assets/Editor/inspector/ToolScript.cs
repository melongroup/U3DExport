using System.Collections.Generic;
using UnityEngine;
public class InspectorMeshInfo{
    public int numBones;
    public int numVertices;
    public int numTriangles;
    public string name;


    public InspectorMeshInfo parser(Renderer renderer){
        this.name = renderer.name;

        Mesh mesh = null;

        if(renderer is MeshRenderer){
            mesh = renderer.GetComponent<MeshFilter>().mesh;
        }else if(renderer is SkinnedMeshRenderer){
            mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
            numBones = (renderer as SkinnedMeshRenderer).bones.Length;
        }

        if(mesh){
            numVertices = mesh.vertices.Length;
            numTriangles = mesh.triangles.Length / 3;
        }


        return this;
    }

    public string toString(){
        return name + "： V：" + numVertices + " T：" + numTriangles + " Bones："+numBones;
    }
}

public class InspectorAvatarInfo{
    public int numNodes = 0;
    public List<InspectorMeshInfo> list;
    public GameObject gameObject;

    public InspectorAvatarInfo parser(GameObject gameObject){

        if(this.gameObject == gameObject){
            return this;
        }

        this.gameObject = gameObject;

        numNodes = getNumNodes(gameObject.transform);

        list = getMeshInfoList();

        

        return this;
    }

    public int getNumNodes(Transform transform){
        int count = 1;
        for (int i = 0; i < transform.childCount; i++)
        {
            count += getNumNodes(transform.GetChild(i));
        }
        return count;
    }



    public List<InspectorMeshInfo> getMeshInfoList(){
        list = new List<InspectorMeshInfo>();
        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            list.Add(new InspectorMeshInfo().parser(renderer));
        }

        return list;
    }


    public string toString(){
        return "nodes：" + numNodes + " meshs："+list.Count;
    }

}