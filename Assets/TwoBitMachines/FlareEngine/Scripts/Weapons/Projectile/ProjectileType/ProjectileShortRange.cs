using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu("")]
        public class ProjectileShortRange : ProjectileBase
        {
                [SerializeField] public LayerMask layer;
                [SerializeField] public float hitRate;
                [SerializeField] public OnTriggerRelease release;
                [SerializeField] public BoxCollider2D boxCollider;
                [SerializeField] public UnityEvent onFire;

                [System.NonSerialized] public float hitCounter;
                [System.NonSerialized] public bool activated = false;
                [System.NonSerialized] public bool wasActivated = false;
                [System.NonSerialized] private List<Collider2D> contactResults = new List<Collider2D>();
                [System.NonSerialized] private ContactFilter2D contactFilter = new ContactFilter2D();

                [System.NonSerialized] private FlameThrower flame;

                public void Start ()
                {
                        ammunition.RestoreValue();
                        contactFilter.useLayerMask = true;
                        contactFilter.layerMask = layer;
                        gameObject.SetActive(false);
                        flame = this.gameObject.GetComponent<FlameThrower>();
                }

                public override void Activate (bool value)
                {
                        gameObject.SetActive(value);
                }

                public override void ResetAll ()
                {
                        hitCounter = 0;
                        activated = false;
                        wasActivated = false;
                        gameObject.SetActive(false);
                }

                public override void Execute ()
                {
                        if (!activated && !wasActivated)
                        {
                                return;
                        }
                        if (wasActivated && !activated && this.gameObject.activeInHierarchy && release == OnTriggerRelease.DeactivateGameObject) // call only once firearm stops firing
                        {
                                triggerReleased = true;
                                if (flame == null)
                                        gameObject.SetActive(false);
                        }
                        wasActivated = activated;
                        activated = false;
                }

                public override bool FireProjectile (Vector2 positionRef, Quaternion rotationRef, Vector2 playerVelocityRef)
                {
                        playerVelocity = playerVelocityRef;
                        if (!ammunition.Consume(pattern.projectileRate, inventory))
                        {
                                return false;
                        }
                        activated = true;
                        transform.position = positionRef;
                        transform.rotation = rotationRef;
                        AreaScan(rotationRef * Vector3.right);
                        gameObject.SetActive(true);
                        onFire.Invoke();
                        return true;
                }

                public void AreaScan (Vector2 direction)
                {
                        if (boxCollider != null && Clock.Timer(ref hitCounter, hitRate))
                        {
                                int hits = boxCollider.OverlapCollider(contactFilter, contactResults);
                                for (int i = 0; i < hits; i++)
                                {
                                        if (contactResults[i] != null)
                                        {
                                                Health.IncrementHealth(transform, contactResults[i].transform, -damage, direction * damageForce);
                                        }
                                }
                        }
                }

        }
}
