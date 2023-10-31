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
        public class RailSlide : Ability
        {
                [SerializeField] public float speedBoost = 2f;
                [SerializeField] public float crouchBoost = 1.5f;
                [SerializeField] public bool crouchSpeedBoost = false;
                [SerializeField] public bool forceDirection = false;

                [System.NonSerialized] private float directionX = 1f;
                [System.NonSerialized] private float counter;
                [System.NonSerialized] private bool exitTime;
                [System.NonSerialized] private bool crouching;
                [System.NonSerialized] private bool sliding;

                public override void Reset (AbilityManager player)
                {
                        sliding = false;
                        exitTime = false;
                        counter = 0;
                }

                public override bool TurnOffAbility (AbilityManager player)
                {

                        Reset(player);
                        return true;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        if (pause)
                        {
                                return false;
                        }
                        if (sliding && player.world.onGround && !SlideOnLayer(player))
                        {
                                exitTime = true;
                        }
                        if (sliding)
                        {
                                return true;
                        }
                        if (SlideOnLayer(player))
                        {
                                Reset(player);
                                return sliding = true;
                        }
                        return false;
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        bool onRail = SlideOnLayer(player);
                        float totalBoost = speedBoost;
                        float speed = player.walk.speed;

                        if (crouchSpeedBoost)
                        {
                                if (player.world.boxCollider.size.y != player.world.box.boxSize.y || crouching)
                                {
                                        crouching = true;
                                        totalBoost += crouchBoost;
                                }
                                if (crouching && player.world.onGround && (player.world.boxCollider.size.y == player.world.box.boxSize.y))
                                {
                                        crouching = false;
                                }
                        }

                        if (exitTime && onRail)
                        {
                                exitTime = false;
                                counter = 0;
                        }
                        if (exitTime)
                        {
                                counter += Time.deltaTime;
                                float percent = (counter / 0.5f);
                                velocity.x = Mathf.Lerp(directionX * totalBoost * speed, directionX * speed, percent);
                                if (percent >= 1)
                                {
                                        Reset(player);
                                }
                        }
                        else
                        {
                                velocity.x = directionX * totalBoost * speed;
                        }
                        if (velocity.x != 0)
                        {
                                player.UpdateVelocityGround();
                        }
                        player.dashBoost = totalBoost;
                        player.signals.Set("onRail");
                        player.lockVelX = true;
                }

                public bool SlideOnLayer (AbilityManager player)
                {
                        if (player.world.verticalTransform != null)// force direction from left to right, or right to left
                        {
                                bool found = false;
                                if (player.world.verticalTransform.gameObject.CompareTag("RailRight"))
                                {
                                        directionX = 1f;
                                        found = true;
                                }
                                else if (player.world.verticalTransform.gameObject.CompareTag("RailLeft"))
                                {
                                        directionX = -1f;
                                        found = true;
                                }
                                if (found && !forceDirection)
                                {
                                        directionX = player.inputX != 0 ? Mathf.Sign(player.inputX) : player.playerDirection;
                                }
                                return found;
                        }
                        return false;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open(parent, "Rail Slide", barColor, labelColor))
                        {
                                FoldOut.Box(3, FoldOut.boxColorLight, yOffset: -2);
                                {
                                        parent.Field("Speed Boost", "speedBoost");
                                        parent.FieldAndEnable("Crouch Boost", "crouchBoost", "crouchSpeedBoost");
                                        parent.FieldToggleAndEnable("Force Direction", "forceDirection");
                                }
                                Layout.VerticalSpacing(3);
                        }
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }
}
