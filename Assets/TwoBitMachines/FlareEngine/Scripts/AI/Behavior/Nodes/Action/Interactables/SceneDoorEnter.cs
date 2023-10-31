#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu("")]
        public class SceneDoorEnter : Conditional
        {
                [SerializeField] public Player player;
                [SerializeField] public float targetX = 5f;
                [SerializeField] public int doorIndex = 0;
                [SerializeField] public float velocity = 10f;

                [SerializeField] private float direction = 1f;
                [SerializeField] private bool isActive = false;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                isActive = false;
                                if (player == null)
                                {
                                        player = ThePlayer.Player.mainPlayer;
                                }
                        }
                        if (player == null || WorldManager.get == null)
                        {
                                return NodeState.Failure;
                        }
                        if (!isActive)
                        {
                                int currentIndex = WorldManager.get.save.sceneDoor;
                                if (Mathf.Abs(currentIndex) != doorIndex)
                                {
                                        return NodeState.Failure;
                                }

                                isActive = true;
                                WorldManager.get.save.sceneDoor = 0;
                                player.BlockInput(true);
                                direction = Mathf.Sign(currentIndex);
                                Vector3 location = this.transform.position;
                                player.transform.position = new Vector3(location.x , location.y , player.transform.position.z);
                                Safire2DCamera.Safire2DCamera.ResetCameras();
                        }
                        if (isActive && Time.deltaTime != 0)
                        {
                                player.BlockInput(true);
                                float destination = this.transform.position.x + targetX * direction;
                                float start = player.transform.position.x;
                                float newPosition = Mathf.MoveTowards(start , destination , velocity * Time.deltaTime);
                                float newVelocity = (newPosition - start) / Time.deltaTime;
                                player.Control(newVelocity , false , false);

                                if (Mathf.Abs(destination - start) <= 0.001f)
                                {
                                        player.BlockInput(false);
                                        return NodeState.Success;
                                }
                        }
                        return NodeState.Running;
                }

                public override void OnReset (bool skip = false , bool enteredState = false)
                {
                        if (player != null)
                                player.BlockInput(false);
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(110 , "After entering a scene, the player will be placed at the door's position, then move towards target x with the specified velocity if the door has the correct door index. No door index should have a value of zero. If player is empty, the system will use the first player it finds." +
                                        "\n \nReturns Running, Failure, Success");
                        }

                        FoldOut.Box(3 , color , yOffset: -2);
                        parent.Field("Door Index" , "doorIndex");
                        parent.FieldDouble("Target X" , "targetX" , "velocity");
                        Labels.FieldDoubleText("Position" , "Velocity");
                        parent.Field("Player" , "player");
                        Layout.VerticalSpacing(3);
                        return true;
                }

                public override void OnSceneGUI (UnityEditor.Editor editor)
                {
                        Draw.GLStart();
                        Draw.GLCircle(this.transform.position , 0.25f , Color.red);
                        Draw.GLCircle(this.transform.position + Vector3.right * targetX , 0.25f , Color.green);
                        Draw.GLCircle(this.transform.position - Vector3.right * targetX , 0.25f , Color.green);
                        Draw.GLEnd();
                }

#pragma warning restore 0414
#endif
                #endregion

        }

}
