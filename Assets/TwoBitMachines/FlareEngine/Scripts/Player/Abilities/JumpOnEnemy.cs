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
        public class JumpOnEnemy : Ability
        {
                [SerializeField] public LayerMask layer;
                [SerializeField] public float damage = 1f;
                [SerializeField] public float damageForce = 1f;
                [SerializeField] public float bounceForce = 10f;
                [SerializeField] public float bounceForceBoost = 1f;
                [SerializeField] public string bounceWE;
                [SerializeField] public string boostButton;
                [SerializeField] public string avoidTag;
                [SerializeField] public UnityEventEffect onBounce;

                [System.NonSerialized] private Health health;
                private void Awake ()
                {
                        health = this.gameObject.GetComponent<Health>();
                }

                public override void EarlyExecute (AbilityManager player , ref Vector2 velocity) // since using EarlyExecute, this will ALWAYS execute. No need for priority.
                {
                        if (pause || player.onVehicle || player.ground || velocity.y > 0 || (health != null && health.GetValue() == 0))
                                return;

                        Vector2 v = velocity * Time.deltaTime;
                        BoxInfo box = player.world.box;
                        float magnitude = Mathf.Abs(v.y) + box.skin.y + 0.1f;
                        Vector2 corner = box.bottomLeft;

                        for (int i = 0; i < box.rays.y; i++)
                        {
                                Vector2 origin = corner + box.right * (box.spacing.x * i + v.x);
                                RaycastHit2D hit = Physics2D.Raycast(origin , -box.up , magnitude , layer);

                                #region Debug
#if UNITY_EDITOR
                                if (WorldManager.viewDebugger)
                                {
                                        Debug.DrawRay(origin , -box.up * magnitude , Color.blue);
                                }
#endif
                                #endregion

                                if (hit && avoidTag != "" && avoidTag != "Untagged" && hit.transform.gameObject.CompareTag(avoidTag))
                                {
                                        continue;
                                }

                                if (hit && damage != 0 && Health.IncrementHealth(transform , hit.transform , -damage , box.down * damageForce))
                                {
                                        float boost = player.inputs.Holding(boostButton) ? bounceForceBoost : 1f;
                                        velocity.y = bounceForce * boost;
                                        onBounce.Invoke(ImpactPacket.impact.Set(bounceWE , hit.transform , hit.collider , hit.transform.position , transform , player.world.box.down , -damage));
                                        //Physics2D.IgnoreCollision(player.world.boxCollider, hit.collider, true);
                                        return;
                                }
                        }
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                [SerializeField, HideInInspector] private bool bounceFoldOut;
                public override bool OnInspector (SerializedObject controller , SerializedObject parent , string[] inputList , Color barColor , Color labelColor)
                {
                        if (Open(parent , "Jump On Enemy" , barColor , labelColor))
                        {
                                FoldOut.Box(4 , FoldOut.boxColorLight , yOffset: -2);
                                {
                                        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
                                        parent.Field("Layer" , "layer");
                                        parent.Field("Damage" , "damage");
                                        parent.Field("Damage Force" , "damageForce");
                                        //parent.Field("Avoid Tag", "avoidTag");
                                        parent.DropDownList(tags , "Avoid Tag" , "avoidTag");
                                }
                                Layout.VerticalSpacing(3);

                                FoldOut.Box(2 , FoldOut.boxColorLight);
                                {
                                        parent.DropDownList(inputList , "Boost Button" , "boostButton");
                                        parent.FieldDouble("Bounce Force" , "bounceForce" , "bounceForceBoost");
                                        Labels.FieldText("Boost" , rightSpacing: 2);
                                }
                                Layout.VerticalSpacing(5);

                                Fields.EventFoldOutEffect(parent.Get("onBounce") , parent.Get("bounceWE") , parent.Get("bounceFoldOut") , "On Bounce" , color: FoldOut.boxColorLight);
                        }
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion

        }
}