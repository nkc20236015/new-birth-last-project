#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
        [System.Serializable]
        public class Peek
        {
                [SerializeField] public bool enable;
                [SerializeField] public bool ignoreClamps;
                [SerializeField] public float time = 0.25f;
                [SerializeField] public float distance = 5f;

                [SerializeField] public KeyCode buttonA = KeyCode.W;
                [SerializeField] public KeyCode buttonB = KeyCode.S;
                [SerializeField] public Vector2 directionA = Vector2.up;
                [SerializeField] public Vector2 directionB = Vector2.down;

                [System.NonSerialized] private Vector2 tempDirection;
                [System.NonSerialized] private float activeDistance;
                [System.NonSerialized] private float counter;
                [System.NonSerialized] private bool tempPeek;

                public enum MousePeekType
                {
                        Button,
                        Automatic
                }

                public void Reset ()
                {
                        counter = 0;
                        activeDistance = 0;
                        tempDirection = Vector2.zero;
                        tempPeek = false;
                }

                public void Velocity (Camera camera , Follow follow)
                {
                        if (!enable || !ignoreClamps || follow.isUser || follow.highlightTarget.active || follow.usingAutoRail)
                                return;

                        Vector3 appliedPeek = Vector2.zero;
                        if (Input.GetKey(buttonA) && !Input.GetKey(buttonB))
                        {
                                appliedPeek = PeekNow(follow , directionA.normalized);
                        }
                        else if (Input.GetKey(buttonB) && !Input.GetKey(buttonA))
                        {
                                appliedPeek = PeekNow(follow , directionB.normalized);
                        }
                        else if (tempPeek)
                        {
                                tempPeek = false;
                                appliedPeek = PeekNow(follow , tempDirection.normalized);
                        }
                        else if (counter > 0)
                        {
                                appliedPeek = PeekReverse(follow);
                        }
                        follow.SetCameraPosition(camera.transform.position + appliedPeek);
                }

                public Vector3 Velocity (Follow follow , bool isUser)
                {
                        if (!enable || ignoreClamps || isUser || follow.usingAutoRail)
                                return Vector3.zero;

                        if (Input.GetKey(buttonA) && !Input.GetKey(buttonB))
                        {
                                return PeekNow(follow , directionA.normalized);
                        }
                        else if (Input.GetKey(buttonB) && !Input.GetKey(buttonA))
                        {
                                return PeekNow(follow , directionB.normalized);
                        }
                        else if (tempPeek)
                        {
                                tempPeek = false;
                                return PeekNow(follow , tempDirection.normalized);
                        }
                        else if (counter > 0)
                        {
                                return PeekReverse(follow);
                        }
                        return Vector3.zero;
                }

                private Vector2 PeekReverse (Follow follow)
                {
                        follow.ForceTargetClamp(tempDirection.x != 0 , tempDirection.y != 0);
                        Clock.TimerReverse(ref counter , 0);
                        activeDistance = distance * (counter / time);

                        if (counter == 0 && tempDirection.y != 0)
                        {
                                follow.ForceTargetSmooth(x: false);
                        }
                        if (counter == 0 && tempDirection.x != 0)
                        {
                                follow.ForceTargetSmooth(y: false);
                        }

                        return tempDirection.normalized * activeDistance;
                }

                private Vector2 PeekNow (Follow follow , Vector2 direction)
                {
                        follow.ForceTargetClamp(direction.x != 0 , direction.y != 0);
                        tempDirection = direction;
                        Clock.TimerExpired(ref counter , time);
                        activeDistance = distance * (counter / time);
                        return direction * activeDistance;
                }

                public void PeekUp ()
                {
                        tempPeek = true;
                        tempDirection = Vector2.up;
                }

                public void PeekDown ()
                {
                        tempPeek = true;
                        tempDirection = Vector2.down;
                }

                public void PeekRight ()
                {
                        tempPeek = true;
                        tempDirection = Vector2.right;
                }

                public void PeekLeft ()
                {
                        tempPeek = true;
                        tempDirection = Vector2.left;
                }

                public void PeekDirection (Vector2 direction)
                {
                        tempPeek = true;
                        tempDirection = direction;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOut;
                [SerializeField, HideInInspector] public bool close;
                [SerializeField, HideInInspector] public bool edit;
                [SerializeField, HideInInspector] public bool add;

                public static void CustomInspector (SerializedProperty parent , Color barColor , Color labelColor)
                {
                        if (!parent.Bool("edit"))
                                return;

                        if (Follow.Open(parent , "Peek" , barColor , labelColor))
                        {
                                GUI.enabled = parent.Bool("enable");
                                FoldOut.Box(3 , Tint.Box);
                                parent.Field("Distance" , "distance");
                                parent.Field("Easing Time" , "time");
                                parent.Field("Ignore Clamps" , "ignoreClamps");
                                Layout.VerticalSpacing(5);

                                FoldOut.Box(2 , Tint.Box);
                                parent.FieldDouble("Direction A" , "buttonA" , "directionA");
                                parent.FieldDouble("Direction B" , "buttonB" , "directionB");
                                Layout.VerticalSpacing(5);
                                GUI.enabled = true;
                        };
                }
#pragma warning restore 0414
#endif
                #endregion
        }
}
