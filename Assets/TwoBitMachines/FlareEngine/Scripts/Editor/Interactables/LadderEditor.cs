using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor(typeof(Ladder))]
        [CanEditMultipleObjects]
        public class LadderEditor : UnityEditor.Editor
        {
                private Ladder main;
                private SerializedObject so;

                private void OnEnable ()
                {
                        main = target as Ladder;
                        so = serializedObject;
                        Layout.Initialize();
                }

                public override void OnInspectorGUI ()
                {
                        Layout.Update();
                        Layout.VerticalSpacing(10);
                        so.Update();


                        if (Block.Header(so).Style(Tint.Box).Fold("Ladder").Build())
                        {
                                SerializedProperty ladder = this.so.Get("ladder");
                                Block.Box(5, FoldOut.boxColor, noGap: true);
                                {
                                        ladder.Field_("Size", "size");
                                        ladder.FieldToggleAndEnable_("Stand On Top", "standOnLadder");
                                        ladder.FieldToggleAndEnable_("Align To Center", "alignToCenter");
                                        ladder.FieldToggleAndEnable_("Can Jump Up", "canJumpUp");
                                        ladder.FieldToggleAndEnable_("Stop Side Jump", "stopSideJump");
                                }
                        }

                        SerializedProperty fence = this.so.Get("fenceFlip");
                        if (Block.Header(fence).Style(Tint.Box).Fold("Fence Flip", "fenceFoldOut").Enable("canFlip").Build())
                        {
                                Block.Box(2, FoldOut.boxColor, noGap: true);
                                {
                                        fence.Field_("Flip time", "flipTime");
                                        fence.Field_("SpriteEngine", "spriteEngine");
                                }
                                GUI.enabled = true;
                        }

                        if (Block.Header(so).Style(Tint.Box).Fold("Fence Reverse", "reverseFoldOut").Enable("canFlip").Build())
                        {
                                Block.Box(3, FoldOut.boxColor, noGap: true);
                                {
                                        so.Field_("Reverse Time", "reverseTime");
                                        so.FieldToggleAndEnable_("Flip X", "canFlipX");
                                        so.FieldToggleAndEnable_("Flip Y", "canFlipY");
                                }
                                GUI.enabled = true;
                        }


                        so.ApplyModifiedProperties();
                        Layout.VerticalSpacing(10);
                }

        }
}

// using TwoBitMachines.FlareEngine.Interactables;
// using UnityEditor;
// using UnityEngine;
// using UnityEditor.UIElements;
// using UnityEngine.UIElements;
// using TwoBitMachines.UIElement.Editor;

// namespace TwoBitMachines.FlareEngine.Editors
// {
//         [CustomEditor(typeof(Ladder))]
//         [CanEditMultipleObjects]
//         public class LadderEditor : UnityEditor.Editor
//         {
//                 public Ladder main;
//                 public VisualElement root;
//                 public SerializedObject so;

//                 private void OnEnable ()
//                 {
//                         main = target as Ladder;
//                         so = serializedObject;
//                         root = ElementTools.InitializeRoot();
//                 }

//                 public override VisualElement CreateInspectorGUI ()
//                 {
//                         SerializedProperty ladderProperty = so.Get("ladder");
//                         var ladder = new BarPlus(root, so, ladderProperty, Tint.BoxDarkGrey)
//                                 .FoldOut("Ladder", Color.black, 0, 1).GetBarPlus();

//                         var block = new Block(ladder.content, so, ladderProperty, Tint.BoxGrey, yOffset: 1); //Template.Box(ladder.content, Tint.BoxGrey, yOffset: 1);
//                         {
//                                 block.Field("Size", "size");
//                                 block.FieldToggle("Stand On Top", "standOnLadder");
//                                 block.FieldToggle("Align To Center", "alignToCenter");
//                                 block.FieldToggle("Can Jump Up", "canJumpUp");
//                                 block.FieldToggle("Stop Side Jump", "stopSideJump");
//                         }

//                         SerializedProperty fenceProperty = so.Get("fenceFlip");
//                         var fence = new BarPlus(root, so, fenceProperty, Tint.BoxDarkGrey)
//                                 .FoldOut("Fence Flip", Color.black, 0, 1, "fenceFoldOut")
//                                 .Enable("canFlip").GetBarPlus();

//                         block = new Block(fence.content, so, fenceProperty, Tint.BoxGrey, yOffset: 1);
//                         {
//                                 block.Field("Flip Time", "flipTime");
//                                 block.Field("Sprite Engine", "spriteEngine");
//                         }

//                         var fenceReverseProperty = new BarPlus(root, so, so, Tint.BoxDarkGrey)
//                                  .FoldOut("Fence Reverse", Color.black, 0, 1, "reverseFoldOut")
//                                  .Enable("canFlip").GetBarPlus();

//                         block = new Block(fenceReverseProperty.content, so, fenceReverseProperty, Tint.BoxGrey, yOffset: 1);
//                         {
//                                 block.Field("Reverse Time", "reverseTime");
//                                 block.FieldToggle("Flip X", "canFlipX");
//                                 block.FieldToggle("Flip Y", "canFlipY");
//                         }

//                         return root;
//                 }

//         }
// }
