#region
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu("")]
        public class CrouchSlide : Ability
        {
                [SerializeField] public SlideType type;
                [SerializeField] public string button;
                [SerializeField] public float height = 1f;
                [SerializeField] public float offset = 0f;
                [SerializeField] public float minTime = 0.25f;
                [SerializeField] public float maxTime = 1f;
                [SerializeField] public float threshold = 1f;
                [SerializeField] public float speedBoost = 1f;

                [System.NonSerialized] private bool isSliding;
                [System.NonSerialized] private bool releasedEarly;
                [System.NonSerialized] private float lerpTime;
                [System.NonSerialized] private int direction;

                public enum SlideType
                {
                        ConstantVelocity,
                        SlowDown
                }

                public override void Reset (AbilityManager player)
                {
                        isSliding = releasedEarly = false;
                        if (player.world.boxCollider.size.y != player.world.box.boxSize.y)
                        {
                                player.world.box.ColliderReset();
                        }
                }

                public override bool TurnOffAbility (AbilityManager player)
                {
                        if (player.world.boxCollider.size.y == player.world.box.boxSize.y || !isSliding)
                        {
                                isSliding = releasedEarly = false;
                                return true;
                        }
                        else if (SafelyStandUp(player.world.box))
                        {
                                isSliding = releasedEarly = false;
                                player.world.box.ColliderReset();
                                return true;
                        }
                        return false;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        if (pause)
                                return false;

                        if (isSliding)
                        {
                                return true;
                        }
                        if (Mathf.Abs(velocity.x) >= threshold && player.ground && !Crouch.IsEdge2D(player.world))
                        {
                                if (player.inputs.Pressed(button) && button == "Jump")
                                {
                                        if (player.world.boxCollider.size.y == player.world.box.boxSize.y)
                                        {
                                                return false;
                                        }
                                }
                                if (player.inputs.Pressed(button))
                                {
                                        return true;
                                }
                                if (player.inputs.Holding(button) && !player.world.wasOnGround)
                                {
                                        return true; // slide after a jump landing
                                }
                        }
                        return false;
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        if (player.world.boxCollider.size.y != height) // CROUCH
                        {
                                lerpTime = 0;
                                isSliding = true;
                                releasedEarly = false;
                                direction = player.playerDirection;
                                player.world.box.ChangeColliderHeight(height, -offset);
                        }
                        if (isSliding)
                        {
                                Slide(player, ref velocity);
                        }
                }

                private void Slide (AbilityManager player, ref Vector2 velocity)
                {
                        float oldVelX = velocity.x;
                        float targetSpeed = type == SlideType.ConstantVelocity ? direction * player.speed : 0;
                        velocity.x = Compute.Lerp(direction * player.speed, targetSpeed, maxTime, ref lerpTime);
                        velocity.x *= speedBoost;

                        player.signals.Set("crouchSlide");

                        if (!player.inputs.Holding(button) && lerpTime >= minTime)
                        {
                                releasedEarly = true;
                        }
                        if (lerpTime >= maxTime || releasedEarly || !player.ground)
                        {
                                player.StopRun();

                                if (SafelyStandUp(player.world.box))
                                {
                                        isSliding = false;
                                        player.world.box.ColliderReset();
                                }
                                else
                                {
                                        velocity.x = oldVelX * 0.5f;
                                        player.signals.Set("crouch");
                                        player.signals.Set("crouchWalk", velocity.x != 0);
                                }
                        }
                        if (velocity.x != 0)
                        {
                                player.UpdateVelocityGround();
                        }
                }

                public bool SafelyStandUp (BoxInfo ray)
                {
                        float length = Mathf.Abs(ray.boxSize.y - ray.collider.size.y) * ray.collider.transform.localScale.y;

                        for (int i = 0; i < ray.rays.y; i++)
                        {
                                Vector2 origin = ray.cornerTopLeft + ray.right * (ray.spacing.x * i);
                                #region Debug
#if UNITY_EDITOR
                                if (WorldManager.viewDebugger)
                                {
                                        Debug.DrawRay(origin, ray.up * length, Color.white);
                                }
#endif
                                #endregion
                                RaycastHit2D hit = Physics2D.Raycast(origin, ray.up, length, WorldManager.collisionMask);
                                if (hit && hit.distance == 0 && hit.transform.gameObject.layer == WorldManager.platformLayer)
                                {
                                        continue;
                                }
                                if (hit)
                                {
                                        return false;
                                }
                        }
                        return true;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open(parent, "Crouch Slide", barColor, labelColor))
                        {
                                FoldOut.Box(4, FoldOut.boxColorLight, yOffset: -2);
                                {
                                        parent.DropDownList(inputList, "Button", "button");
                                        parent.Field("Slide Type", "type");
                                        parent.FieldDouble("Crouch Height", "height", "offset");
                                        Labels.FieldText("Offset");
                                        parent.FieldDouble("Slide Time", "minTime", "maxTime");
                                        Labels.FieldDoubleText("Min", "Max", rightSpacing: 1);
                                }
                                Layout.VerticalSpacing(3);

                                FoldOut.Box(2, FoldOut.boxColorLight);
                                {
                                        parent.Field("Speed Threshold", "threshold");
                                        parent.Field("Speed Boost", "speedBoost");
                                }
                                Layout.VerticalSpacing(5);
                        }
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion

        }
}
