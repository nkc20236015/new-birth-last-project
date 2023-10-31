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
        public class SceneDoorExit : Conditional
        {
                [SerializeField] public Player player;
                [SerializeField] public ManageScenes manageScenes;
                [SerializeField] public SceneDoorDirection directional;
                [SerializeField] public string sceneName;
                [SerializeField] public int doorIndex;
                [SerializeField] public Vector3 area;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                if (player == null)
                                {
                                        player = ThePlayer.Player.mainPlayer;
                                }
                        }
                        if (player == null || manageScenes == null || WorldManager.get == null)
                        {
                                return NodeState.Failure;
                        }
                        if (!InsideArea(player.transform.position + Vector3.up * 0.1f))
                        {
                                return NodeState.Failure;
                        }

                        int direction = directional == SceneDoorDirection.Fixed ? 1 : (int) Mathf.Sign(player.abilities.playerDirection);
                        WorldManager.get.save.sceneDoor = doorIndex * direction;
                        manageScenes.LoadScene(sceneName);
                        return NodeState.Success;
                }

                private bool InsideArea (Vector2 target)
                {
                        Vector2 position = this.transform.position - Vector3.right * area.z;
                        if (target.x > (position.x + (area.x * 0.5f)) || target.x < (position.x - (area.x * 0.5f)))
                        {
                                return false;
                        }
                        if (target.y > (position.y + area.y) || target.y < position.y)
                        {
                                return false;
                        }
                        return true;
                }

                public enum SceneDoorDirection
                {
                        Fixed,
                        Directional
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                public static string[] sceneNames = new string[] { "Empty" };

                public override bool OnInspector (AIBase ai , SerializedObject parent , Color color , bool onEnable)
                {
                        if (onEnable)
                        {
                                int sceneCount = Util.SceneCount();
                                if (sceneCount > 0)
                                {
                                        sceneNames = new string[sceneCount];
                                        for (int i = 0; i < sceneCount; i++)
                                        {
                                                sceneNames[i] = Util.GetSceneName(i);
                                        }
                                }
                                else
                                {
                                        Debug.Log("Include scenes into Build Settings to use SceneDoorExit");
                                }
                        }
                        if (parent.Bool("showInfo"))
                        {
                                Labels.InfoBoxTop(125 ,
                                        "If the player enters the door area, the specified scene will load by calling the manage scenes component." +
                                        " If Door Entry is Directional, the player will enter the next door moving in the direction it exit." +
                                        " The area's z value is treated as an x offset for the door area. If player is empty, the system will use the first player it finds." +
                                        "\n \nReturns Failure, Success");
                        }

                        FoldOut.Box(5 , color , yOffset: -2);
                        parent.Field("Door Index" , "doorIndex");
                        parent.Field("Door Area" , "area");
                        parent.Field("Door Entry" , "directional");
                        parent.FieldAndDropDownList(sceneNames , "Manage Scenes" , "manageScenes" , "sceneName");
                        parent.Field("Player" , "player");
                        Layout.VerticalSpacing(3);
                        return true;
                }

                public override void OnSceneGUI (UnityEditor.Editor editor)
                {
                        Draw.Square(transform.position - Vector3.right * (area.x * 0.5f + area.z) , area , Color.blue);
                }

#pragma warning restore 0414
#endif
                #endregion

        }

}
