#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class FlipChildrenObjects : Action
        {
                [SerializeField] public FlipOn flipOn;
                [SerializeField] public bool flipScaleX = true;
                [SerializeField] public bool flipPositionX = false;

                [SerializeField] public float positionXDirection = 1f;
                [SerializeField] public float scaleXDirection = 1f;
                [SerializeField, HideInInspector] public SpriteRenderer spriteRenderer;
                [System.NonSerialized] public float oldSign;

                public override NodeState RunNodeLogic (Root root)
                {
                        float sign = Mathf.Sign(root.direction);
                        if (root.velocity.x == 0)
                                sign = oldSign;

                        if (flipOn == FlipOn.SpriteDirection)
                        {
                                if (spriteRenderer == null)
                                        spriteRenderer = transform.gameObject.GetComponent<SpriteRenderer>();
                                if (spriteRenderer != null)
                                        sign = spriteRenderer.flipX ? -1f : 1f;
                        }

                        for (int i = 0; i < this.transform.childCount; i++)
                        {
                                if (flipPositionX)
                                {
                                        Vector3 p = this.transform.GetChild(i).localPosition;
                                        this.transform.GetChild(i).localPosition = new Vector3(sign * positionXDirection > 0 ? Mathf.Abs(p.x) : -Mathf.Abs(p.x) , p.y , p.z);
                                }
                                if (flipScaleX)
                                {
                                        float scaleX = Mathf.Abs(this.transform.GetChild(i).localScale.x) * sign * scaleXDirection;
                                        this.transform.GetChild(i).localScale = new Vector3(scaleX , this.transform.GetChild(i).localScale.y , this.transform.GetChild(i).localScale.z);
                                }
                        }
                        oldSign = sign;
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
                                Labels.InfoBoxTop(65 , "Flip the children objects of this AI in the x direction. In an AIFSM, run this inside an Always state." +
                                        "\n \nReturns Running");
                        }

                        FoldOut.Box(3 , color , yOffset: -2);
                        parent.Field("Flip On" , "flipOn");
                        parent.FieldAndEnable("Flip Scale X" , "scaleXDirection" , "flipScaleX");
                        Labels.FieldText("Direction" , rightSpacing: 18);
                        parent.FieldAndEnable("Flip Position X" , "positionXDirection" , "flipPositionX");
                        Labels.FieldText("Direction" , rightSpacing: 18);
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }

        public enum FlipOn
        {
                VelocityDirection,
                SpriteDirection
        }

}
