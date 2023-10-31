using System;
using System.Reflection;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CanEditMultipleObjects, CustomEditor(typeof(AudioSFXGroup))]
        public class AudioSFXGroupEditor : UnityEditor.Editor
        {
                private SerializedObject parent;
                public static string inputName = "Name";

                private void OnEnable ()
                {
                        parent = serializedObject;
                        Layout.Initialize();
                }

                public override void OnInspectorGUI ()
                {
                        Layout.Update();
                        Layout.VerticalSpacing(10);

                        parent.Update();
                        {
                                FoldOut.BoxSingle(3 , Tint.BoxTwo);
                                {
                                        parent.Field("Audio Manager" , "audioManager");
                                        parent.Field("Time Rate" , "timeRate");
                                        parent.FieldAndEnable("Attenuate" , "distance" , "attenuate");
                                }
                                Layout.VerticalSpacing(2);

                                SerializedProperty sfx = parent.Get("sfx");

                                for (int i = 0; i < sfx.arraySize; i++)
                                {
                                        SerializedProperty element = sfx.Element(i);
                                        float width = Layout.labelWidth + Layout.contentWidth - 67;

                                        FoldOut.Bar(element , Tint.BoxTwo , 0)
                                                .Grip(parent , sfx , i , color: Tint.Grey)
                                                .LF("clip" , (int) (width * 0.69f) , -4 , 2)
                                                .LF("volume" , (int) (width * 0.275f) , -4 , 2);
                                        element.Clamp("volume");

                                        ListReorder.Grip(parent , sfx , Layout.GetLastRect(20 , 20) , i , Tint.WarmWhite , yOffset: 1);

                                        if (Bar.ButtonRight("Delete" , Tint.White))
                                        {
                                                sfx.MoveArrayElement(i , sfx.arraySize - 1);
                                                sfx.arraySize--;
                                                break;
                                        }
                                        if (Bar.ButtonRight("Play" , Tint.White))
                                        {
                                                StopAllClips();
                                                PlayClip(element.Get("clip").objectReferenceValue as AudioClip);
                                        }
                                        if (Bar.ButtonRight("Red" , Tint.White))
                                        {
                                                StopAllClips();
                                        }
                                }

                                if (FoldOut.CornerButton(Tint.Blue))
                                {
                                        sfx.arraySize++;
                                        sfx.LastElement().Get("volume").floatValue = 1f;
                                }

                        }
                        parent.ApplyModifiedProperties();
                        Layout.VerticalSpacing(10);

                }

                //* https: //forum.unity.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/
                public static void PlayClip (AudioClip clip , int startSample = 0 , bool loop = false)
                {
                        if (clip == null)
                        {
                                return;
                        }
                        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

                        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
                        MethodInfo method = audioUtilClass.GetMethod(
                                "PlayPreviewClip" ,
                                BindingFlags.Static | BindingFlags.Public ,
                                null ,
                                new Type[] { typeof(AudioClip) , typeof(int) , typeof(bool) } ,
                                null
                        );

                        method.Invoke(null , new object[] { clip , startSample , loop });
                }

                public static void StopAllClips ()
                {
                        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

                        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
                        MethodInfo method = audioUtilClass.GetMethod(
                                "StopAllPreviewClips" ,
                                BindingFlags.Static | BindingFlags.Public ,
                                null ,
                                new Type[] { } ,
                                null
                        );
                        method.Invoke(null , new object[] { });
                }

        }
}
