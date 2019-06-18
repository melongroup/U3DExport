using System;
using UnityEngine;

namespace rf
{
    public class LightParser : ComponentParser
    {

        public override object parser(GameObject _object, Component component, UnityScene element){
            // base.writeData
            


            return true;
        }

        // public override bool writeData(GameObject obj, Component component,ExportConfig config)
        // {
        //     SkinnedMeshRenderer comp = component as SkinnedMeshRenderer;
        //     compJson.SetBool("_castShadows", comp.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off);
        //     compJson.SetBool("_receiveShadows", comp.receiveShadows);
        //     compJson.SetMesh(obj, comp.sharedMesh);
        //     compJson.SetMaterials(obj, comp.sharedMaterials, false, true);

        //     return true;
        // }
    }
}