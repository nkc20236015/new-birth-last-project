using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateLadder : FollowerState
        {
                public override void Execute (TargetPathfindingBase ai, ref Vector2 velocity)
                {
                        RemoveMoveSafelyY(ai);
                        RemoveOutOfOrderY(ai);
                        if (ai.currentNode.Same(ai.nextNode) && ai.futureNode != null && !ai.futureNode.isOccupied)
                        {
                                if (!ai.nextNode.SameX(ai.futureNode))
                                {
                                        Jump(ai, ref velocity);
                                        return;
                                }
                                if (!ai.nextNode.ground && ai.futureNode.ground)
                                {
                                        JumpFall(ai, ref velocity);
                                        return;
                                }
                        }
                        if (ai.state == PathState.Ladder && !ai.stateChanged && !ai.currentNode.ladder)
                        {
                                ai.state = PathState.Follow;
                                ai.CalculatePath(ai.targetRef);
                                return;
                        }

                        velocity = Vector2.zero;
                        ai.signals.Set("ladderClimb", true);
                        MoveToTarget(ai.position.x, ai.nextNode.position.x, ai.followSpeed * 0.5f, ref velocity.x);
                        MoveToTarget(ai.position.y, ai.nextNode.position.y, ai.ladderSpeed, ref velocity.y);
                        //  Debug.Log("Vel y " + velocity.y + "  " + ai.position.y + "  " + ai.nextNode.position.y);
                }

                public static void FindLadderClimb (TargetPathfindingBase ai, ref Vector2 velocity)
                {
                        if (ai.nextNode.ladder && ai.currentNode.SameX(ai.nextNode) && ai.futureNode.ladder && !ai.futureNode.ground)
                        {
                                ai.state = PathState.Ladder;
                        }
                }
        }
}
///(ai.nextNode == null || !ai.nextNode.ladder))
// if (!ai.OnGround() || (ai.currentNode.ladder && ai.nextNode.gridY > ai.currentNode.gridY))
// {

// }

// if (StateMoving.WaitForMovingPlatform(ai, this, ref velocity))
//                                 {
//                                         ai.SetAnimation("ladderClimb", ai.state == PathState.Ladder);
//                                         return;
//                                 }
//                                 else 
// private void ClimbLadder (TargetPathfindingBase ai, ref Vector2 velocity)
// {
//         velocity = Vector2.zero;
//         RemoveMoveSafelyY(ai);
//         MoveToTarget(ai.position.x, ai.nextNode.position.x, ai.followSpeed * 0.5f, ref velocity.x);
//         if ((ai.currentNode.ladder && ai.nextNode.gridY > ai.currentNode.gridY) || !ai.OnGround())
//         {
//                 ai.SetAnimation("ladderClimb", true);
//                 MoveToTarget(ai.position.y, ai.nextNode.position.y, ai.ladderSpeed, ref velocity.y);
//         }
// }
