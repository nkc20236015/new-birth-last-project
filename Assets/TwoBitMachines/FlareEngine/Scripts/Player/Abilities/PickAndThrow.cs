#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu("")]
        public class PickAndThrow : Ability
        {
                [SerializeField] public string grabButton;
                [SerializeField] public string dropButton;
                [SerializeField] public float throwTime = 0.5f;
                [SerializeField] public float pickUpTime = 0.25f;
                [SerializeField] public bool pickUpLerp = true;
                [SerializeField] public Vector2 holdPosition = new Vector2(0 , 2f);
                [SerializeField] public List<Vector2> pickUpPath = new List<Vector2>();

                [System.NonSerialized] private AIBase block;
                [System.NonSerialized] private PickState state;
                [System.NonSerialized] private int pickUpIndex;
                [System.NonSerialized] private float throwCounter;
                [System.NonSerialized] private float pickUpCounter;
                [System.NonSerialized] private Vector2 currentPath;
                [System.NonSerialized] private BoxCollider2D blockCollider;
                [System.NonSerialized] private Transform blockTransform;

                public enum PickState
                {
                        None,
                        PickingUp,
                        Holding,
                        Throwing
                }

                public override void Reset (AbilityManager player)
                {
                        state = PickState.None;
                        blockTransform = null;
                        pickUpCounter = pickUpIndex = 0;
                        if (block != null)
                        {
                                blockCollider.gameObject.layer = WorldManager.platformLayer;
                                block.root.pauseCollision = false;
                                block.world.ResetCollisionInfo();
                                block.world.Reset();
                                block.world.box.Update();
                                blockCollider = null;
                                block = null;
                        }
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

                        if (state == PickState.PickingUp || state == PickState.Holding || state == PickState.Throwing)
                        {
                                return true;
                        }
                        if (velocity.x != 0 || !player.ground)
                        {
                                blockTransform = null;
                        }
                        if (player.world.onWall)
                        {
                                blockTransform = player.world.wallTransform;
                        }
                        if (player.world.onGround && player.inputs.Holding("Down") && player.inputs.Holding(grabButton) && block == null && blockCollider == null)
                        {
                                if (player.world.verticalTransform != null && player.world.verticalTransform.CompareTag("Block"))
                                {
                                        return BeginHold(player.world.verticalTransform , player);
                                }
                        }
                        if (player.inputs.Holding(grabButton) && block == null && blockCollider == null && blockTransform != null && blockTransform.CompareTag("Block"))
                        {
                                return BeginHold(blockTransform , player);
                        }
                        return false;
                }

                private bool BeginHold (Transform transform , AbilityManager player)
                {

                        pickUpCounter = pickUpIndex = 0;
                        block = transform.GetComponent<AIBase>();
                        blockCollider = transform.GetComponent<BoxCollider2D>();
                        bool blockIsValid = block != null && blockCollider != null;
                        if (blockIsValid)
                                state = PickState.PickingUp;
                        currentPath = !blockIsValid ? Vector2.zero : (Vector2) block.transform.position - player.world.position;
                        return blockIsValid;
                }

                public override void ExecuteAbility (AbilityManager player , ref Vector2 velocity , bool isRunningAsException = false)
                {
                        player.signals.Set("pickAndThrowBlock");
                        player.signals.Set("pickingUpBlock" , state == PickState.PickingUp);
                        player.signals.Set("holdingBlock" , state == PickState.Holding);
                        player.signals.Set("throwingBlock" , state == PickState.Throwing);
                }

                public override void PostCollisionExecute (AbilityManager player , Vector2 velocity)
                {
                        switch (state)
                        {
                                case PickState.None:
                                        break;
                                case PickState.PickingUp:
                                        if (block == null || blockCollider == null || !block.gameObject.activeInHierarchy)
                                        {
                                                Reset(player);
                                                break;
                                        }
                                        if ((pickUpPath.Count == 0 || pickUpTime == 0) && HoldBlock(player , velocity))
                                        {
                                                blockCollider.gameObject.layer = 2;
                                                state = PickState.Holding; // hold here for one frame so press doesn't execute immediately
                                                break;
                                        }

                                        if (blockCollider == null || blockCollider.gameObject == null)
                                        {
                                                break;
                                        }

                                        blockCollider.gameObject.layer = 2;
                                        float time = pickUpTime / pickUpPath.Count;

                                        if (pickUpLerp)
                                        {
                                                pickUpCounter = Mathf.Clamp(pickUpCounter + Time.deltaTime , 0 , time);
                                                Vector2 newPosition = Compute.LerpNormal(currentPath , pickUpPath[pickUpIndex] , pickUpCounter / time);
                                                newPosition.x = player.playerDirection > 0 ? Mathf.Abs(newPosition.x) : -Mathf.Abs(newPosition.x);
                                                block.transform.position = player.world.position + newPosition;
                                                if (pickUpCounter >= time)
                                                {
                                                        currentPath = pickUpPath[pickUpIndex];
                                                        pickUpCounter = 0;
                                                        pickUpIndex++;
                                                }
                                        }
                                        else
                                        {
                                                if (Clock.Timer(ref pickUpCounter , time))
                                                {
                                                        currentPath = pickUpPath[pickUpIndex];
                                                        pickUpIndex++;
                                                }
                                                currentPath.x = player.playerDirection > 0 ? Mathf.Abs(currentPath.x) : -Mathf.Abs(currentPath.x);
                                                block.transform.position = player.world.position + currentPath;
                                        }
                                        if (pickUpIndex >= pickUpPath.Count)
                                        {
                                                pickUpCounter = pickUpIndex = 0;
                                                state = PickState.Holding;
                                        }
                                        break;
                                case PickState.Holding:
                                        if (block == null || blockCollider == null || !block.gameObject.activeInHierarchy || BlockHitWall(velocity))
                                        {
                                                Reset(player);
                                        }
                                        else if (HoldBlock(player , velocity))
                                        {
                                                if (player.inputs.Pressed(grabButton) & Time.deltaTime != 0)
                                                {
                                                        player.signals.Set("throwingBlock");
                                                        state = PickState.Throwing;
                                                        throwCounter = 0;

                                                        Throw jumpTo = block.GetComponent<Throw>();
                                                        if (jumpTo != null)
                                                        {
                                                                block.ChangeState("Throw");
                                                                jumpTo.SetForce(velocity / Time.deltaTime);
                                                                block.root.signals.characterDirection = player.playerDirection;
                                                                Reset(player);
                                                        }
                                                }
                                                else if (player.inputs.Pressed(dropButton))
                                                {
                                                        throwCounter = 0;
                                                        block.root.signals.characterDirection = player.playerDirection;
                                                        Reset(player);
                                                }
                                        }
                                        break;
                                case PickState.Throwing:
                                        if (Clock.Timer(ref throwCounter , throwTime))
                                        {
                                                Reset(player);
                                        }
                                        break;
                        }
                }

                private bool HoldBlock (AbilityManager player , Vector2 velocity)
                {
                        block.root.pauseCollision = true;
                        Vector2 oldPosition = block.transform.position;
                        Vector2 newHoldPosition = holdPosition;
                        newHoldPosition.x *= Mathf.Sign(player.playerDirection);
                        block.transform.position = player.world.position + newHoldPosition;

                        if (BoxInfo.BoxTouching(blockCollider , WorldManager.collisionMask , 0.0015f , 0.0015f))
                        {
                                block.transform.position = oldPosition;
                                Reset(player);
                                return false;
                        }
                        return true;
                }

                private bool BlockHitWall (Vector2 velocity)
                {
                        if (blockCollider == null)
                                return false;

                        BoxInfo.GetColliderCorners(blockCollider);

                        bool hitX = false;
                        Vector2 updateSpeed = Vector3.zero;
                        float signX = Mathf.Sign(velocity.x);
                        float magnitudeX = Mathf.Abs(velocity.x);
                        Vector2 topCorner = signX > 0 ? BoxInfo.topRightCorner : BoxInfo.topLeftCorner;
                        Vector2 bottomCorner = signX > 0 ? BoxInfo.bottomRightCorner : BoxInfo.bottomLeftCorner;

                        for (int i = 0; i < 2; i++)
                        {
                                Vector2 origin = i == 0 ? bottomCorner : topCorner;
                                RaycastHit2D hit = Physics2D.Raycast(origin , blockCollider.transform.right * signX , magnitudeX , WorldManager.worldMask);
                                //Debug.DrawRay (origin, blockCollider.transform.right * signX * magnitudeX, Color.red);
                                if (hit && hit.transform != this.transform)
                                {
                                        hitX = true;
                                        magnitudeX = hit.distance;
                                }
                        }
                        if (hitX)
                        {
                                updateSpeed = Vector3.right * signX * (magnitudeX - 0.0015f);
                                block.transform.position += (Vector3) updateSpeed;
                        }

                        float magnitudeY = Mathf.Abs(velocity.y);
                        Vector2 bottomRCorner = BoxInfo.bottomLeftCorner + updateSpeed;
                        Vector2 bottomLCorner = BoxInfo.bottomRightCorner + updateSpeed;
                        bool hitY = false;

                        for (int i = 0; i < 2; i++)
                        {
                                Vector2 origin = i == 0 ? bottomLCorner : bottomRCorner;
                                RaycastHit2D hit = Physics2D.Raycast(origin , -blockCollider.transform.up , magnitudeY , WorldManager.worldMask);
                                // Debug.DrawRay (origin, -blockCollider.transform.up * magnitudeY, Color.red);
                                if (hit && hit.transform != this.transform)
                                {
                                        hitY = true;
                                        magnitudeY = hit.distance;
                                }
                        }
                        if (hitY)
                        {
                                block.transform.position += Vector3.down * magnitudeY;
                        }

                        return hitX || hitY;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414

                public override bool OnInspector (SerializedObject controller , SerializedObject parent , string[] inputList , Color barColor , Color labelColor)
                {
                        if (Open(parent , "Pick And Throw" , barColor , labelColor))
                        {
                                FoldOut.Box(3 , FoldOut.boxColorLight , yOffset: -2);
                                {
                                        parent.DropDownList(inputList , "Grab Button" , "grabButton");
                                        parent.DropDownList(inputList , "Drop Button" , "dropButton");
                                        parent.Field("Throw Time" , "throwTime");
                                }
                                Layout.VerticalSpacing(3);

                                FoldOut.Box(3 , FoldOut.boxColorLight);
                                {
                                        parent.Field("Hold Position" , "holdPosition");
                                        parent.Field("Pick Up Time" , "pickUpTime");
                                        parent.FieldToggle("Lerp Pick Up" , "pickUpLerp");
                                }
                                Layout.VerticalSpacing(5);

                                SerializedProperty array = parent.Get("pickUpPath");
                                if (array.arraySize == 0)
                                {
                                        array.arraySize++;
                                }

                                FoldOut.Box(array.arraySize , FoldOut.boxColorLight);
                                {
                                        parent.Get("pickUpPath").FieldProperty("Pick Up Path");
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
