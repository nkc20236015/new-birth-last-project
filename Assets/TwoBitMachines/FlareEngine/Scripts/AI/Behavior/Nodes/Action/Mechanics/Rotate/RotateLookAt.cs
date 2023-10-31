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
        public class RotateLookAt : Action
        {
                [SerializeField] public Blackboard lookAt;
                [SerializeField] public float speed;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (lookAt == null)
                        {
                                return NodeState.Failure;
                        }

                        if (Time.deltaTime != 0)
                        {
                                Vector3 dir = (Vector3) lookAt.GetTarget() - transform.position;
                                float angle = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
                                transform.eulerAngles = new Vector3(0 , 0 , angle);
                        }
                        return NodeState.Running;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool HasNextState () { return false; }
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(45 , "Rotate to look at the specified target." +
                                        "\n \nReturns Running, Failure");
                        }

                        FoldOut.Box(1 , color , yOffset: -2);
                        AIBase.SetRef(ai.data , parent.Get("lookAt") , 0);
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }
}
