using System.Collections.Generic;
using System;
using UnityEngine;

namespace rf
{
    public class MeshRendererParser : ComponentParser
    {

        protected Mesh getMesh(Renderer renderer){
            if(renderer is MeshRenderer){
                return renderer.GetComponent<MeshFilter>().mesh; 
            }else if(renderer is SkinnedMeshRenderer){
                return (renderer as SkinnedMeshRenderer).sharedMesh;
            }
            return null;
        }

        public override object parser(GameObject _object, Component component, UnityScene unityScene){

            Renderer renderer = component as Renderer;
            Mesh mesh = getMesh(renderer);

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

            meshData.bounds = Export.toUntiyBounds(mesh.bounds,new Matrix4x4());

            // meshData.skeleton = skeletonData;


            meshData.material = Export.ExportMaterial(renderer.material);

            meshData.node = renderer.gameObject.name;

            if( "" == meshData.node ){
                Debug.LogError("error node name");
            }



            return meshData;
        }
    }
}