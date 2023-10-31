#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class FieldOfView : Conditional
        {
                [SerializeField] public float radius = 5;
                [SerializeField] public float yOffset = 1;
                [SerializeField] public float angleOffset = -90f;
                [SerializeField] public float positiveAngle = 45f;
                [SerializeField] public float negativeAngle = -45f;
                [SerializeField] public bool flipToX = true;
                [SerializeField] public Blackboard target;

                private int xDirection = 0;

                public override NodeState RunNodeLogic (Root root) //, IStateAI controller, AIState state)
                {
                        if (target == null)
                                return NodeState.Failure;

                        xDirection = root.direction;
                        Vector2 pointOfInterest = target.GetTarget();
                        Vector2 up = transform.up;
                        Vector2 position = root.position + up * yOffset;

                        if ((pointOfInterest - position).sqrMagnitude > radius * radius)
                                return NodeState.Failure;

                        if (Find(pointOfInterest , position , up , positiveAngle + angleOffset , negativeAngle + angleOffset))
                        {
                                return NodeState.Success;
                        }
                        return NodeState.Failure;
                }

                public bool Find (Vector2 target , Vector2 center , Vector2 direction , float angleP , float angleN)
                {
                        Vector2 V1 = Compute.RotateVector(direction , angleP);
                        Vector2 V2 = Compute.RotateVector(direction , angleN);
                        if (flipToX && xDirection < 0)
                        {
                                V1.x *= -1f;
                                V2.x *= -1f;
                        }
                        float angleArea = Vector3.Angle(V1 , V2);
                        return Vector3.Angle(V1 , target - center) < angleArea && Vector3.Angle(V2 , target - center) < angleArea;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override void OnSceneGUI (UnityEditor.Editor editor)
                {
                        float sign = flipToX && xDirection < 0 ? -1f : 1f;
                        float angleA = positiveAngle + angleOffset;
                        float angleB = negativeAngle + angleOffset;
                        Draw.CircleSector(transform.position + transform.up * yOffset , transform.up , radius , angleA , angleB , sign , radius * 2f);
                }
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(55 , "Returns Success if the specified target is inside the field of view. If Flip to X is enabled, the field of view will flip to the x direction of the AI.");
                        }

                        FoldOut.Box(7 , color , yOffset: -2);
                        AIBase.SetRef(ai.data , parent.Get("target") , 0);
                        parent.Field("Length" , "radius");
                        parent.Field("Angle Offset" , "angleOffset");
                        parent.Field("Positive Angle" , "positiveAngle");
                        parent.Field("Negative Angle" , "negativeAngle");
                        parent.Field("Y Offset" , "yOffset");
                        parent.Field("Flip To X" , "flipToX");
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }
}
