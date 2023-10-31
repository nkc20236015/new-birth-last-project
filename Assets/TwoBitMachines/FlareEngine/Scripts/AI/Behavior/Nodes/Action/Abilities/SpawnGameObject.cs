#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class SpawnGameObject : Action
        {
#pragma warning disable 0108
                [SerializeField] public GameObject gameObject;
#pragma warning restore 0108
                [SerializeField] public bool recycle;
                [SerializeField] public Transform parent;
                [SerializeField] public PositionType type;
                [SerializeField] public Blackboard target;
                [SerializeField] public Vector2 position;
                [System.NonSerialized] private List<GameObject> objs = new List<GameObject>();

                public override NodeState RunNodeLogic (Root root)
                {
                        if (gameObject == null || (type == PositionType.Target && target == null))
                        {
                                return NodeState.Failure;
                        }
                        if (recycle)
                        {
                                for (int i = 0; i < objs.Count; i++)
                                {
                                        if (objs[i] != null && !objs[i].activeInHierarchy)
                                        {
                                                objs[i].transform.position = type == PositionType.Point ? position : target.GetTarget();
                                                objs[i].transform.rotation = Quaternion.identity;
                                                objs[i].SetActive(true);
                                                return NodeState.Success;
                                        }
                                }
                        }
                        GameObject obj;
                        if (type == PositionType.Point)
                        {
                                obj = Instantiate(gameObject , position , Quaternion.identity , parent != null ? parent : transform);
                                obj.SetActive(true);
                        }
                        else
                        {
                                obj = Instantiate(gameObject , target.GetTarget() , Quaternion.identity , parent != null ? parent : transform);
                                obj.SetActive(true);
                        }
                        if (recycle && !objs.Contains(obj))
                        {
                                objs.Add(obj);
                        }
                        return NodeState.Success;
                }

                public enum PositionType
                {
                        Point,
                        Target
                }

                #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(60 , "Spawn a gameObject at the specified position. If recycle is enabled, a pool will be created for the objects." +
                                        "\n \nReturns Success, Failure");
                        }

                        int type = parent.Enum("type");

                        FoldOut.Box(5 , color , yOffset: -2);
                        parent.Field("Game Object" , "gameObject");
                        parent.Field("Type" , "type");
                        parent.Field("Point" , "position" , execute: type == 0);
                        if (type == 1)
                                AIBase.SetRef(ai.data , parent.Get("target") , 0);
                        parent.Field("Parent" , "parent");
                        parent.FieldToggle("Recycle" , "recycle");
                        return true;
                }
#pragma warning restore 0414
#endif
                #endregion
        }

}
