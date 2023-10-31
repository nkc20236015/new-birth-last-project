using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TwoBitMachines.FlareEngine
{
        [RequireComponent (typeof (TextMeshProEffects))]
        public class Bubble : MonoBehaviour, ITypeWriterComplete
        {
                [SerializeField] public Image image;
                [SerializeField] public TMP_Text text;
                [SerializeField] public bool isDirectional;
                [SerializeField] public Vector2 maxSize = new Vector2 (6f, 6f);
                [SerializeField] public Vector2 minSize = new Vector2 (2f, 1f);
                [SerializeField] public Vector2 padding = new Vector2 (0.5f, 0.5f);
                [SerializeField] public float directionXOffset = 0f;
                [SerializeField] public float offsetY = 0.1f;

                [SerializeField] public UnityEvent transitionIn = new UnityEvent ( );
                [SerializeField] public UnityEvent transitionOut = new UnityEvent ( );
                [SerializeField] public UnityEvent messageLoaded = new UnityEvent ( );

                [System.NonSerialized] private Vector2 size;
                [System.NonSerialized] private RectTransform textRect;
                [System.NonSerialized] private RectTransform containerRect;
                [System.NonSerialized] private DialogueBubble parent;
                [System.NonSerialized] private Choice choice;
                [System.NonSerialized] private TextMeshProEffects effects;
                public bool messageLoadingComplete { get; private set; }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool inFoldOut;
                [SerializeField, HideInInspector] private bool outFoldOut;
                [SerializeField, HideInInspector] private bool loadFoldOut;
                [SerializeField, HideInInspector] private bool eventsFoldOut;
                #pragma warning restore 0414
                #endif
                #endregion

                public void TransitionIn (string message, float fadeTime, Tween tween, float messageDirection, Choice choiceRef = null, DialogueBubble parentRef = null)
                {
                        choice = choiceRef;
                        parent = parentRef;
                        messageLoadingComplete = false;
                        if (textRect == null)
                        {
                                textRect = text?.GetComponent<RectTransform> ( );
                        }
                        if (containerRect == null)
                        {
                                containerRect = image?.GetComponent<RectTransform> ( );
                        }
                        if (text != null && textRect != null && containerRect != null)
                        {
                                text.SetText (message);
                                size = text.GetPreferredValues (maxSize.x, maxSize.y);
                                size.x = Mathf.Clamp (size.x, minSize.x, maxSize.x);
                                size.y = Mathf.Clamp (size.y, minSize.y, maxSize.y);

                                containerRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, size.x + padding.x);
                                containerRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, size.y + padding.y);
                                textRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, size.x);
                                textRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, size.y);
                                textRect.anchoredPosition = new Vector2 (0, offsetY);
                                size += padding;
                        }
                        if (!gameObject.activeInHierarchy)
                        {
                                gameObject.SetActive (true);
                        }
                        if (effects == null)
                        {
                                effects = gameObject.GetComponent<TextMeshProEffects> ( );
                        }

                        Flip (messageDirection);
                        transitionIn.Invoke ( );
                        effects?.BeginTextMeshEffects ( );
                        Wiggle.Target (gameObject).StartScale (Vector3.zero).ScaleTo2D (Vector2.one, fadeTime, tween);
                }

                public void TransitionOut (float fadeTime, Tween tween)
                {
                        if (gameObject.activeInHierarchy)
                        {
                                if (text != null)
                                {
                                        text.SetText ("");
                                }
                                Wiggle.Target (gameObject).StartScale (Vector3.one).ScaleTo2D (Vector2.zero, fadeTime, tween).Deactivate ( );
                                transitionOut.Invoke ( );
                        }
                }

                public void TypingComplete ( )
                {
                        messageLoadingComplete = true;
                        messageLoaded.Invoke ( );
                        if (parent != null && parent.message != null)
                        {
                                parent.message.onMessage.Invoke ( );
                        }
                }

                public void TypingCommence ( )
                {
                        messageLoadingComplete = false;
                }

                private void Flip (float direction)
                {
                        if (isDirectional && direction != 0)
                        {
                                image.transform.localScale = new Vector3 (direction, 1f, 1f);
                                Vector3 lp = transform.localPosition;
                                transform.localPosition = new Vector3 (Mathf.Abs (size.x * 0.5f + directionXOffset) * direction, lp.y, lp.z);
                        }
                }

                public void OnSelect ( )
                {
                        if (choice != null && parent != null)
                        {
                                choice.dialogue = null;
                                choice.dialogueBubble = parent;
                                choice.ChoiceSelected ( );
                        }
                }
        }

}