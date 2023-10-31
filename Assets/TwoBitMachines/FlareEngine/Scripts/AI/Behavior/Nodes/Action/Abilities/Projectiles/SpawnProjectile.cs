#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class SpawnProjectile : Action
        {
                [SerializeField] public ProjectileBase projectile;
                [SerializeField] public Vector2 direction;
                [SerializeField] public PositionType type;
                [SerializeField] public Blackboard target;
                [SerializeField] public Vector2 position;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (projectile == null || (type == PositionType.Target && target == null))
                                return NodeState.Failure;

                        if (type == PositionType.Point)
                        {
                                projectile.FireProjectileStatic(position , direction);
                        }
                        else if (type == PositionType.Target)
                        {
                                projectile.FireProjectileStatic(target.GetTarget() , direction);
                        }
                        else
                        {
                                projectile.FireProjectileStatic(transform.position , direction);
                        }
                        return NodeState.Success;
                }

                public enum PositionType
                {
                        Point,
                        Target,
                        AI_Position
                }

                #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(100 , "Spawn a projectile at the fire point. If direction is enabled, the projectile will move in the specified direction; otherwise, the direction will be relative to the ai and fire point in the x direction." +
                                        "\n \nReturns Success, Failure");
                        }

                        int type = parent.Enum("type");
                        int height = type == 2 ? -1 : 0;
                        FoldOut.Box(4 + height , color , yOffset: -2);
                        {
                                parent.Field("Projectile" , "projectile");
                                parent.Field("Fire Point" , "type");
                                parent.Field("Point" , "position" , execute: type == 0);
                                if (type == 1)
                                        AIBase.SetRef(ai.data , parent.Get("target") , 0);
                                parent.Field("Direction" , "direction");
                        }
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }

}
