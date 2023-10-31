using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/一WorldEvents/WorldEvent")]
        public class WorldEventTrigger : MonoBehaviour
        {
                public WorldEventSO worldEvent;

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField] private bool foldOut = false;
                [SerializeField] private string eventName = "";
                #pragma warning restore 0414
                #endif
                #endregion

                public void TriggerEvent ( ) // Call this to trigger the world event, 
                {
                        if (worldEvent != null)
                                worldEvent.TriggerEvent ( );
                }
        }
}