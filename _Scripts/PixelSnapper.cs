using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// Makes the current object inline with with the pixels of the tilemap if enabled.
    /// </summary>
    public class PixelSnapper : MonoBehaviour
    {
        /// <summary>
        /// Snaps the givne position to the pixel grid.
        /// </summary>
        /// <param name="position">The position to snap.</param>
        /// <returns>The snapped position.</returns>
        public static Vector3 Snap(Vector3 position)
        {
            return new Vector3(Mathf.Round(16 * position.x) / 16, Mathf.Round(16 * position.y) / 16, 0);
        }

        void Awake()
        {
            transform.position = Snap(transform.position);
        }
    }
}
