using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu("")]
        public class TargetCharacter : Blackboard
        {
                [SerializeField] public LayerMask layer;
                [SerializeField] public Vector2 offset;

                public override Vector2 GetTarget (int index = 0)
                {
                        Vector2 position = transform.position;
                        return NearesPosition(position);
                }

                public Vector2 NearesPosition (Vector2 returnPosition)
                {
                        List<WorldCollision> npc = Character.characters;

                        if (npc.Count > 0)
                        {
                                float distance = float.MaxValue;
                                Vector2 position = returnPosition;
                                for (int i = 0; i < npc.Count; i++)
                                {
                                        if (npc[i] == null || npc[i].transform == this.transform || !Compute.ContainsLayer(layer, npc[i].transform.gameObject.layer))
                                        {
                                                continue;
                                        }
                                        float sqrMag = (returnPosition - (Vector2) npc[i].transform.position).sqrMagnitude;
                                        if (sqrMag < distance)
                                        {
                                                distance = sqrMag;
                                                position = npc[i].transform.position;
                                        }
                                }
                                return position + offset;
                        }
                        return returnPosition;
                }

                private Transform NearestTransform (Vector2 position)
                {
                        List<WorldCollision> npc = Character.characters;

                        if (npc.Count > 0)
                        {
                                float distance = float.MaxValue;
                                Transform newTransform = null;
                                for (int i = 0; i < npc.Count; i++)
                                {
                                        if (npc[i] == null || npc[i].transform == this.transform || !Compute.ContainsLayer(layer, npc[i].transform.gameObject.layer))
                                        {
                                                continue;
                                        }
                                        float sqrMag = (position - (Vector2) npc[i].transform.position).sqrMagnitude;
                                        if (sqrMag < distance)
                                        {
                                                distance = sqrMag;
                                                newTransform = npc[i].transform;
                                        }
                                }
                                return newTransform;
                        }
                        return null;
                }

                public override Transform GetTransform ()
                {
                        return NearestTransform(transform.position);
                }

                private Transform GetNPCTransform ()
                {
                        return NearestTransform(transform.position);
                }

                public override void Set (Vector3 vector3)
                {
                        Transform transform = GetNPCTransform();
                        if (transform != null)
                                transform.position = vector3;
                }

                public override void Set (Vector2 vector2)
                {
                        Transform transform = GetNPCTransform();
                        if (transform != null)
                                transform.position = vector2;
                }

                public override Vector2 GetOffset ()
                {
                        return offset;
                }
        }

}
