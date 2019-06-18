using System;
using UnityEngine;
namespace rf
{
    /**
     * 组件管理器接口
     */
    public interface IComponentParser
    {
        Type compType { set; get; }
        // string className { set; get; }

        object parser(GameObject _object,Component component,UnityScene unityScene);
    }

    public abstract class ComponentParser : IComponentParser
    {
        protected Type _compType;
        protected string _className;
        public virtual object parser(GameObject _object, Component component, UnityScene unityScene)
        {
            return null;
        }

        public Type compType { get { return _compType; } set { _compType = value; } }
        // public string className { get { return _className; } set { _className = value; }  }


        protected string getRefName(string path){
            path = path.ToLower().Replace("assets/","");
            int i = path.IndexOf(".");
            if(i != -1){
                path = path.Substring(0,i);
            }
            return path;
        }
    }
}