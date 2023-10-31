using UnityEngine;

namespace TwoBitMachines.FlareEngine.BulletType
{
    [AddComponentMenu("")]
    public class Seeker : BulletBase
    {
        [SerializeField] public float searchRadius = 20f;
        [SerializeField] public float searchRate = 0.25f;
        [SerializeField] public float turnSpeed = 2; // between 0 and 1
        [SerializeField] public int bulletRays = 1;
        [SerializeField] public Vector2 bulletSize = Vector2.one;
        [SerializeField] public ProjectileSeekerType find;
        [SerializeField] private bool addMomentum = false;

        [System.NonSerialized] private Collider2D target = null;
        [System.NonSerialized] private float searchRateCounter;
        private bool targetEmpty => target == null || !target.enabled || target.transform == null || !target.gameObject.activeInHierarchy;

        public override void OnReset(Vector2 characterVelocity)
        {
            searchRateCounter = 0;
            target = null;
            AddMomentum(addMomentum, characterVelocity * 0.5f);
        }

        public override void Execute()
        {
            if (SetToSleep())
            {
                return;
            }
            LifeSpanTimer();
            ApplyRotation(this.transform);
            Follow();
            CollisionDetection(bulletRays, bulletSize);
            transform.position = position;
            transform.rotation = rotation;
        }

        private void Follow()
        {
            if (!targetEmpty)
            {
                FollowTarget(ref position, ref velocity, ref rotation, target.transform.position, turnSpeed);
            }
            else
            {
                position += velocity * Time.deltaTime;
                if (Clock.Timer(ref searchRateCounter, searchRate)) //                        find target
                {
                    int targets = Compute.OverlapCircle(position, searchRadius, layer); // target layer should only have enemy targets
                    if (find == ProjectileSeekerType.RandomTarget)
                    {
                        target = Compute.HitContactRandomResult(targets, position);
                    }
                    else
                    {
                        target = Compute.HitContactNearestResult(targets, position);
                    }
                }
            }
        }

        private void FollowTarget(ref Vector2 position, ref Vector2 velocity, ref Quaternion rotation, Vector2 target, float turnSpeed)
        {
            Vector2 velNormal = velocity.normalized;
            Vector2 direction = (target - position).normalized;
            float rotateDirection = Compute.CrossSign(direction, velNormal);
            velocity = Compute.RotateVector(velocity, (turnSpeed * Time.deltaTime) * -rotateDirection * Vector2.Angle(direction, velNormal));
            position += velocity * Time.deltaTime;
            rotation = Compute.LookAtDirection(velocity);
        }

    }
}