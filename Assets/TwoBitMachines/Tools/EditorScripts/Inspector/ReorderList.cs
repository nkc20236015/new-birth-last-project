#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.Editors
{
        public class ListReorder
        {
                public static int source = 0;
                public static int destination = 0;
                public static float timeStart;

                public static bool Reorder (Rect rect , Texture2D icon , int listIndex , SerializedProperty array , SerializedProperty index , SerializedProperty active , Color color)
                {
                        switch (Event.current.type)
                        {
                                case EventType.Repaint:
                                        if (icon != null)
                                                Skin.DrawTexture(rect , icon , index.intValue == listIndex && active.boolValue ? Tint.Grey150 : color);
                                        break;
                                case EventType.MouseDown:
                                        if (rect.ContainsMouseDown(false))
                                        {
                                                timeStart = Time.time;
                                                active.boolValue = true;
                                                GUI.FocusControl(null);
                                                index.intValue = listIndex;
                                        }
                                        break;
                                case EventType.MouseUp:
                                        if (active.boolValue)
                                        {
                                                active.boolValue = false;
                                                Layout.UseEvent();
                                        }
                                        break;
                                case EventType.MouseDrag:
                                        rect.width = Layout.maxWidth;
                                        if (active.boolValue && rect.ContainsMouse())
                                        {
                                                if (listIndex != index.intValue)
                                                {
                                                        source = index.intValue;
                                                        destination = listIndex;
                                                        array.MoveArrayElement(index.intValue , listIndex);
                                                        index.intValue = listIndex;
                                                        return true;
                                                }
                                                Layout.UseEvent();
                                        }
                                        break;
                        }
                        return false;
                }

                public static bool Grip (SerializedObject property , SerializedProperty array , Rect rect , int index , Color gripColor , string signalIndex = "signalIndex" , string active = "active" , int yOffset = 0)
                {
                        return Grip(property.Get(signalIndex) , property.Get(active) , array , rect , index , gripColor , 0 , yOffset);
                }

                public static bool Grip (SerializedProperty property , SerializedProperty array , Rect rect , int index , Color gripColor , string signalIndex = "signalIndex" , string active = "active" , int yOffset = 0)
                {
                        return Grip(property.Get(signalIndex) , property.Get(active) , array , rect , index , gripColor , 0 , yOffset);
                }

                public static bool Grip (SerializedProperty signalIndex , SerializedProperty active , SerializedProperty array , Rect rect , int index , Color gripColor , float xOffset = 0 , int yOffset = 0)
                {
                        if (signalIndex == null || active == null)
                                return false;
                        Rect grip = new Rect(rect) { x = 12 + xOffset , y = rect.y + 2 + yOffset , width = 12 , height = 12 };
                        return Reorder(grip , Icon.Get("Grip") , index , array , signalIndex , active , gripColor);
                }

                public static bool GripRaw (SerializedObject property , SerializedProperty array , Rect rect , int index , string signalIndex = "signalIndex" , string active = "active")
                {
                        return GripRaw(property.Get(signalIndex) , property.Get(active) , array , rect , index);
                }

                public static bool GripRaw (SerializedProperty signalIndex , SerializedProperty active , SerializedProperty array , Rect rect , int index)
                {
                        if (signalIndex == null || active == null)
                                return false;
                        return Reorder(rect , null , index , array , signalIndex , active , Color.clear);
                }
        }

}
#endif
