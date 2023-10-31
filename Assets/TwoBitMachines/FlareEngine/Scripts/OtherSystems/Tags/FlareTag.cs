using UnityEngine;
using System.Collections.Generic;

namespace TwoBitMachines.FlareEngine
{
        public class FlareTag : MonoBehaviour
        {
                [SerializeField] public TagListSO tagListSO;
                [SerializeField] public List<string> tags = new List<string>();

                public bool Contains (string id)
                {
                        return tags.Contains(id);
                }
        }
}
