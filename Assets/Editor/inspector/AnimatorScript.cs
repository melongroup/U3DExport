using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using rf;

[CustomEditor(typeof(Animator))]
public class Animator_Inspector : Editor
{
    public bool bFuncSrc = false;

    [System.Obsolete]
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        // GUILayout.Space(16);
        var animator = target as Animator;

        if(null == animator){
            return;
        }

        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Controller：",GUILayout.Width(100));
                EditorGUILayout.ObjectField(animator.runtimeAnimatorController,typeof(AnimatorController));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Avatar：",GUILayout.Width(100));
                EditorGUILayout.ObjectField(animator.avatar,typeof(Avatar));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            avatarInfo = getModelInfo(avatarInfo);

            EditorGUILayout.LabelField(avatarInfo.toString());

            EditorGUI.indentLevel++;

            foreach (var item in avatarInfo.list)
            {
                EditorGUILayout.LabelField(item.toString());
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(5);

            if(animator.runtimeAnimatorController){
            
                EditorGUILayout.LabelField("Animation("+animator.runtimeAnimatorController.animationClips.Length+")");

                EditorGUI.indentLevel++;

                foreach (var clip in animator.runtimeAnimatorController.animationClips)
                {
                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(clip.name);
                        if(GUILayout.Button("play",GUILayout.Width(50),GUILayout.Height(20))){
                            animator.Play(clip.name,0);
                            //todo play
                        }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;

                GUILayout.Space(5);

            }

            EditorGUILayout.BeginHorizontal();
                // GUILayout.Button(Export.config.outdir);
                // EditorGUILayout.ObjectField("path",Export.config.outdir,typeof(GUIContent));
                EditorGUILayout.LabelField("path",GUILayout.Width(40));
                EditorGUILayout.TextField(Export.config.outdir);
                if(GUILayout.Button("folder",GUILayout.Width(50),GUILayout.Height(20))){
                    // SerializeObject.Serialize(animator.gameObject);
                    Export.config.outdir = EditorUtility.OpenFolderPanel("当前要导出的路径", Export.config.outdir, "");
                    Setup.save();
                }
                if(GUILayout.Button("export",GUILayout.Width(50),GUILayout.Height(20))){
                    SerializeObject.ExportGameObject(animator.gameObject);
                }
            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        // base.OnInspectorGUI();
    }

    public InspectorAvatarInfo avatarInfo;


    public InspectorAvatarInfo getModelInfo(InspectorAvatarInfo avatarInfo){
        var animator = target as Animator;
        var obj = animator.gameObject;

        if(null == avatarInfo){
            avatarInfo = new InspectorAvatarInfo();
        }

        avatarInfo.parser(obj);

        return avatarInfo;
    }
}