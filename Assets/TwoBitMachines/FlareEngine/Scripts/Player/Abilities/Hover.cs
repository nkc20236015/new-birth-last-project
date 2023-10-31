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
        public class Hover : Ability // anim signal: hover
        {
                [SerializeField] public string thrust;
                [SerializeField] public float hoverThrust = 1;
                [SerializeField] public float maintainThrust = 1;
                [SerializeField] public float descendThrust = 1;
                [SerializeField] public string descend;
                [SerializeField] public float airFrictionX = 0.5f;
                [SerializeField] public ThrustExit exitType;
                [SerializeField] public string exit;
                [SerializeField] public UnityEventEffect onThrust;
                [SerializeField] public UnityEventEffect onDescend;
                [SerializeField] public string thrustWE;
                [SerializeField] public string descendWE;
                [System.NonSerialized] public bool hovering = false;

                #region 
#if UNITY_EDITOR
#pragma warning disable 0414
                [SerializeField] private bool eventsFoldOut = false;
                [SerializeField] private bool thrustFoldOut = false;
                [SerializeField] private bool descendFoldOut = false;
#endif
                #endregion

                public override void Reset (AbilityManager player)
                {
                        hovering = false;
                }

                public override bool TurnOffAbility (AbilityManager player)
                {
                        Reset(player);
                        return true;
                }

                public override bool IsAbilityRequired (AbilityManager player , ref Vector2 velocity)
                {
                        if (pause)
                                return false;

                        bool thrust = player.inputs.Pressed(this.thrust);

                        if (hovering)
                        {
                                if (exitType == ThrustExit.OnGroundHit)
                                {
                                        if (player.ground && !thrust)
                                        {
                                                hovering = false;
                                        }
                                }
                                else if (player.inputs.Pressed(this.exit))
                                {
                                        hovering = false;
                                }
                        }
                        bool isHovering = hovering;
                        hovering = false;
                        return thrust || isHovering;
                }

                public override void ExecuteAbility (AbilityManager player , ref Vector2 velocity , bool isRunningAsException = false)
                {
                        hovering = true;
                        player.world.hitInteractable = true;
                        player.signals.Set("hover" , true);
                        HorizontalVelocity(player.inputX , player.speed , player.velocityX , 1f - airFrictionX , ref velocity); // reapply x velocity, will also override impede change (if it exists)

                        if (player.inputs.Pressed(this.thrust))
                        {
                                velocity.y = player.maxJumpVel * hoverThrust;
                                onThrust.Invoke(ImpactPacket.impact.Set(thrustWE , this.transform , player.world.boxCollider , player.world.position , null , player.world.box.down , 0));

                        }
                        if (player.inputs.Pressed(this.descend))
                        {
                                velocity.y += player.gravityEffect * descendThrust;
                                onDescend.Invoke(ImpactPacket.impact.Set(descendWE , this.transform , player.world.boxCollider , player.world.position , null , player.world.box.down , 0));

                        }
                        if (velocity.y >= 0)
                        {
                                velocity.y -= player.gravityEffect; // undo gravity
                                velocity.y += player.gravityEffect * 0.60f;
                        }
                        else if (velocity.y < 0)
                        {
                                velocity.y -= player.gravityEffect * maintainThrust;
                                velocity.y = velocity.y > 0 ? 0 : velocity.y;
                        }
                }

                public void HorizontalVelocity (float inputX , float speed , float velocityX , float smooth , ref Vector2 velocity)
                {
                        if (smooth > 0 && smooth < 1f)
                        {
                                velocity.x = Compute.Lerp(velocityX , inputX * speed , smooth);
                        }
                        else
                        {
                                velocity.x = inputX * speed;
                        }
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (SerializedObject controller , SerializedObject parent , string[] inputList , Color barColor , Color labelColor)
                {
                        if (Open(parent , "Hover" , barColor , labelColor))
                        {
                                FoldOut.Box(3 , FoldOut.boxColorLight , yOffset: -2);
                                parent.Slider("Thrust" , "hoverThrust" , 0 , 1f);
                                parent.Slider("Maintain" , "maintainThrust" , 0.5f , 1f);
                                parent.DropDownList(inputList , "Thrust Button" , "thrust");
                                Layout.VerticalSpacing(3);

                                FoldOut.Box(2 , FoldOut.boxColorLight);
                                parent.Slider("Descend" , "descendThrust" , 0 , 10f);
                                parent.DropDownList(inputList , "Descend Button" , "descend");
                                Layout.VerticalSpacing(5);

                                FoldOut.Box(2 , FoldOut.boxColorLight , extraHeight: 3);
                                parent.Field("Exit" , "exitType" , execute: parent.Enum("exitType") == 0);
                                parent.FieldAndDropDownList(inputList , "Exit" , "exitType" , "exit" , execute: parent.Enum("exitType") == 1);
                                parent.Slider("Air Friction X" , "airFrictionX" , 0 , 1f);
                                bool eventOpen = FoldOut.FoldOutButton(parent.Get("eventsFoldOut"));

                                Fields.EventFoldOutEffect(parent.Get("onThrust") , parent.Get("thrustWE") , parent.Get("thrustFoldOut") , "On Thrust" , execute: eventOpen , color: FoldOut.boxColorLight);
                                Fields.EventFoldOutEffect(parent.Get("onDescend") , parent.Get("descendWE") , parent.Get("descendFoldOut") , "On Descend" , execute: eventOpen , color: FoldOut.boxColorLight);
                                Layout.VerticalSpacing(1);
                        }
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion

                public enum ThrustExit
                {
                        OnGroundHit,
                        Button
                }
        }
}
