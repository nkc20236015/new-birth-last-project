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
        public class VariableLogic : Conditional
        {
                [SerializeField] public Blackboard variable;
                [SerializeField] public VariableLogicType logic;
                [SerializeField] public CompareTo compareTo;
                [SerializeField] public float compareFloat;
                [SerializeField] public Blackboard compareVariable;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (variable == null)
                                return NodeState.Failure;

                        float compareValue = compareTo == CompareTo.FloatValue ? compareFloat : compareTo == CompareTo.OtherVariable && compareVariable != null ? compareVariable.GetValue() : 0;
                        return Compare(variable.GetValue() , compareValue);
                }

                public NodeState Compare (float a , float b)
                {
                        if (logic == VariableLogicType.Greater)
                        {
                                return a > b ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.Less)
                        {
                                return a < b ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.Equal)
                        {
                                return a == b ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.GreaterOrEqualTo)
                        {
                                return a >= b ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.LessOrEqualTo)
                        {
                                return a <= b ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.IsTrue)
                        {
                                return a > 0 ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.IsFalse)
                        {
                                return a <= 0 ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.IsEven)
                        {
                                return a % 2 == 0 ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.IsOdd)
                        {
                                return a % 2 != 0 ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.Null)
                        {
                                return variable.GetTransform() == null ? NodeState.Success : NodeState.Failure;
                        }
                        if (logic == VariableLogicType.NotNull)
                        {
                                return variable.GetTransform() != null ? NodeState.Success : NodeState.Failure;
                        }
                        return NodeState.Failure;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(35 , "Compare a float variable to a float value.");
                        }

                        int logic = parent.Enum("logic");
                        int height = logic <= 4 ? 2 : 0;
                        int type = parent.Enum("compareTo");
                        FoldOut.Box(2 + height , color , yOffset: -2);
                        AIBase.SetRef(ai.data , parent.Get("variable") , 0);
                        parent.Field("Logic" , "logic");
                        parent.Field("Compare To" , "compareTo" , execute: height == 2);
                        parent.Field("Compare Float" , "compareFloat" , execute: type == 0 && height == 2);
                        if (type == 1 && height == 2)
                                AIBase.SetRef(ai.data , parent.Get("compareVariable") , 1);
                        Layout.VerticalSpacing(3);
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion

        }

        public enum CompareTo
        {
                FloatValue,
                OtherVariable
        }

        public enum VariableLogicType
        {
                Greater,
                Less,
                Equal,
                GreaterOrEqualTo,
                LessOrEqualTo,
                IsTrue,
                IsFalse,
                IsEven,
                IsOdd,
                Null,
                NotNull
        }
}