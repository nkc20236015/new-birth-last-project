#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class ShootProjectileAtTarget : Action
        {
                [SerializeField] public ProjectileBase projectile;
                [SerializeField] public Blackboard target;
                [SerializeField] public Transform firePoint;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (projectile == null || target == null || firePoint == null)
                                return NodeState.Failure;

                        projectile.FireProjectile(transform.position , Rotate() , Vector2.zero);

                        return NodeState.Success;
                }

                private Quaternion Rotate ()
                {
                        Vector2 t = target.GetTarget();
                        float direction = t.x < transform.position.x ? -1f : 1f;
                        Compute.FlipLocalPositionX(firePoint , direction);
                        Vector2 targetNormal = (t - (Vector2) transform.position).normalized;
                        float angle = Compute.AngleDirection(transform.right * direction , targetNormal);
                        float rotateAngle = direction < 0 ? -angle : angle;
                        return Quaternion.Euler(0 , direction < 0 ? 180f : 0f , rotateAngle);
                }

                #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(45 , "Shoot a projectile at a target." +
                                        "\n \nReturns Success, Failure");
                        }

                        FoldOut.Box(3 , color , yOffset: -2);
                        {
                                parent.Field("Projectile" , "projectile");
                                parent.Field("Fire Point" , "firePoint");
                                AIBase.SetRef(ai.data , parent.Get("target") , 0);
                        }
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }

}
