#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class ControlPlayer : Action
        {
                [SerializeField] public Blackboard target;
                [SerializeField] public Player player;
                [SerializeField] public float velocity;
                [SerializeField] public bool unblockOnExit = true;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (player == null || target == null)
                                return NodeState.Failure;

                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                player.BlockInput(true);
                        }
                        if (Time.deltaTime != 0)
                        {
                                float destination = target.GetTarget().x;
                                float start = player.transform.position.x;
                                float newP = Mathf.MoveTowards(start , destination , velocity * Time.deltaTime);
                                float newVelocity = (newP - start) / Time.deltaTime;
                                player.Control(newVelocity , false , false);

                                if (Mathf.Abs(destination - start) <= 0.001f)
                                {
                                        if (unblockOnExit)
                                                player.BlockInput(false);
                                        return NodeState.Success;
                                }
                        }
                        return NodeState.Running;
                }

                public override void OnReset (bool skip = false , bool enteredState = false)
                {
                        if (player != null && unblockOnExit)
                                player.BlockInput(false);
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(65 , "Control the player in the x-direction by moving it to a point. This will also block player input." +
                                        "\n \nReturns Running, Success, Failure");
                        }

                        FoldOut.Box(4 , color , yOffset: -2);
                        AIBase.SetRef(ai.data , parent.Get("target") , 0);
                        parent.Field("Player" , "player");
                        parent.Field("Velocity" , "velocity");
                        parent.Field("Unblock On Exit" , "unblockOnExit");
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }

}