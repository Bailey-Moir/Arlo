using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// Holds sprites and prefabs that can't be retrived through code.
    /// </summary>
    public class GlobalStorage : MonoBehaviour
    {
        /// <summary>
        /// The prefab that holds the health bar the (almost) all entities use.
        /// </summary>
        static public GameObject healthBarPrefab;
        /// <summary>
        /// All of the sprites that spikes loop through when warning the player.
        /// </summary>
        static public Sprite[] spikeWarnings;
        /// <summary>
        /// The spike sprite;
        /// </summary>
        static public Sprite spike;
        /// <summary>
        /// The spike mask sprite;
        /// </summary>
        static public Sprite spikeMask;
        /// <summary>
        /// The ellipsis collider.
        /// </summary>
        static public GameObject ellipsisCollider;

        [SerializeField]
        private GameObject _healthBarPrefab;
        [SerializeField]
        private Sprite[] _spikeWarnings;
        [SerializeField]
        private Sprite _spike;
        [SerializeField]
        private Sprite _spikeMask;
        [SerializeField]
        private GameObject _ellipsisCollider;

        void Awake()
        {
            healthBarPrefab = _healthBarPrefab;
            spikeWarnings = _spikeWarnings;
            spike = _spike;
            spikeMask = _spikeMask;
            ellipsisCollider = _ellipsisCollider;
        }
    }
}
