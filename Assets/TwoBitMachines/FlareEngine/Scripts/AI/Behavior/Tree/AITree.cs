using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("Flare Engine/AITree")]
        public class AITree : AIBase
        {
                [SerializeField] public BoolVariable reset;

                public override void Execute ( )
                {
                        if (root == null || root.pause)
                        {
                                return;
                        }

                        velocity.x = externalVelocity.x;
                        damage.PauseTimer ( );
                        ApplyGravity ( );
                        root.RunTree (ref velocity, ref signals.characterDirection, transform.position);
                        Collision (velocity, ref velocity.y);
                }

                public override void ResetAI ( )
                {
                        if (reset != null)
                        {
                                reset.Set (true);
                        }
                }
        }
}