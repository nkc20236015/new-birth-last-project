using UnityEngine;

namespace TwoBitMachines
{
        public static class DebugTimer
        {
                public static float timeStamp = 0;

                public static void Start ()
                {
                        timeStamp = Time.realtimeSinceStartup;
                }

                public static void Stop (string identifier)
                {
                        float diff = Time.realtimeSinceStartup - timeStamp;
                        timeStamp = Time.realtimeSinceStartup;
                        Debug.Log(identifier + ":  " + diff);
                }
        }

}
