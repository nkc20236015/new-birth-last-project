using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
        public abstract class SpriteEngineBase : MonoBehaviour
        {
                [SerializeField] public SpriteRenderer render;
                [SerializeField] public SpriteTree tree = new SpriteTree();

                [System.NonSerialized] public bool pause;
                [System.NonSerialized] public bool inTransition;
                [System.NonSerialized] public string returnAnimation;
                [System.NonSerialized] public string currentAnimation;

                public static string[] animationDirection = new string[] { "flipLeft", "flipRight", "flipUp", "flipDown", "None" };

                public void OnEnable ()
                {
                        SetFirstAnimation();
                        pause = false;
                }

                public void OnDisable ()
                {
                        pause = true;
                }

                public virtual void Play ()
                {

                }

                public virtual void SetNewAnimation (string newAnimation)
                {

                }

                public virtual bool FlipAnimation (Dictionary<string, bool> signal, string signalName, string direction)
                {
                        return false;
                }

                public virtual void SetFirstAnimation ()
                {

                }

                public void FinalizeAnimation (string newAnimation)
                {
                        SetNewAnimation(newAnimation);
                }

                public void SetSignal (string signalName)
                {
                        tree.SetSignalTrue(signalName);
                }

                public void SetSignalTrue (string signal)
                {
                        tree.SetSignalTrue(signal);
                }

                public void SetSignalFalse (string signal)
                {
                        tree.SetSignalFalse(signal);
                }

                public void SetSignals (Dictionary<string, bool> signals)
                {
                        tree.signal = signals;
                }

                public void SetVelXSignals (float velX)
                {
                        tree.SetSignalTrue("alwaysTrue");
                        tree.Set("velX", velX != 0);
                        tree.Set("velXZero", velX == 0);
                        tree.Set("velXLeft", velX < 0);
                        tree.Set("velXRight", velX > 0);
                }

                public void Pause (bool value)
                {
                        pause = value;
                }
                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
#if UNITY_EDITOR
#pragma warning disable 0414
                [SerializeField, HideInInspector] public bool sort;
                [SerializeField, HideInInspector] private bool add;
                [SerializeField, HideInInspector] private bool active;
                [SerializeField, HideInInspector] private bool foldOut;
                [SerializeField, HideInInspector] private bool open;
                [SerializeField, HideInInspector] private bool delete;
                [SerializeField, HideInInspector] private bool deleteAsk;
                [SerializeField, HideInInspector] private bool spriteMenu;
                [SerializeField, HideInInspector] private bool eventFoldOut;
                [SerializeField, HideInInspector] private bool spritesFoldOut;
                [SerializeField, HideInInspector] private bool loopOnceFoldOut;
                [SerializeField, HideInInspector] private bool transitionFoldOut;
                [SerializeField, HideInInspector] private bool createTransition;
                [SerializeField, HideInInspector] private float shiftNames;
                [SerializeField, HideInInspector] private float shiftProperty;
                [SerializeField, HideInInspector] private int signalIndex;
                [SerializeField, HideInInspector] private int tabIndex;
                [SerializeField, HideInInspector] private bool previewArea = true;

                // for main preview window
                [SerializeField, HideInInspector] private Vector2 scrollPosition = Vector2.zero;
                [SerializeField, HideInInspector] private Vector2 offsetPosition = Vector2.zero;
                [SerializeField, HideInInspector] private Color color = Color.white;
                [SerializeField, HideInInspector] private bool playInInspector;
                [SerializeField, HideInInspector] private bool playInScene;
                [SerializeField, HideInInspector] private bool resetPlayFrame;
                [SerializeField, HideInInspector] private int frameCounter;
                [SerializeField, HideInInspector] private float frameTimer;
                [SerializeField, HideInInspector] private float zoom = 1;

                [SerializeField, HideInInspector] private bool propertyFoldOut;
                [SerializeField, HideInInspector] private bool resetSprites;
                [SerializeField, HideInInspector] private int propertyIndex;
                [SerializeField, HideInInspector] public Sprite rendererSprite;
                [SerializeField, HideInInspector] public bool settingRenderer;
                [SerializeField, HideInInspector] public int spriteIndex;
                [SerializeField, HideInInspector] public int scrollIndex;
                public bool playingInScene => playInScene;

#pragma warning restore 0414
#endif
                #endregion
        }
}
