using System.Collections.Generic;
using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// The class for Items (specifically that used in the Granny level).
    /// </summary>
    public class Item : Interactable
    {
        /// <summary>
        /// Static list of all the items in the scene.
        /// </summary>
        public static List<Item> items = new List<Item>();

        // DEPENDENCIES

        /// <summary>
        /// The GameObject that crosses through the item on the recipe, set by default to inactive.
        /// </summary>
        public GameObject crossThrough;

        /// <summary>
        /// The GameObject for the bowl where all the items will end up.
        /// </summary>
        public GameObject bowl;

        /// <summary>
        /// The Transform of the player in the scene.
        /// </summary>
        private Transform playerTransform;

        /// <summary>
        /// The callback used to move the item to the bowl.
        /// </summary>
        private System.Action _moveCallback;

        void Awake()
        {
            // Add to item list.
            items.Add(this);
            playerTransform = GameObject.Find("Arlo").transform;

            // Set what happens when you interact with it.
            interact = () => {
                // Make it so you can't interact again.
                interactable = false;
                // Create notificatoin.
                UI.Notification.fabricate(name);
                // Cross through the item on the list.
                crossThrough.SetActive(true);

                var time = Time.timeSinceLevelLoad;
                var origin = transform.position;
                var point = bowl.transform.position;
                GetComponent<SpriteRenderer>().sortingOrder = bowl.GetComponent<SpriteRenderer>().sortingOrder + 1;
                
                // Set the move callback to move the item to the bowl using linear interpolation and a special math function I made.
                _moveCallback = () => {
                    var t = Time.timeSinceLevelLoad - time;
                    transform.position = new Vector2(origin.x + (bowl.transform.position.x - origin.x) * t, (4 * t + origin.y) * (1 - t) + point.y * t);
                    if (t >= 1)
                    {
                        transform.position = bowl.transform.position;
                        _moveCallback = null;
                        GameObject.Find("Managers").GetComponent<GrannyScene>().NumberCollected += 1;
                        Destroy(this.gameObject);
                    }
                };
            };
        }
        override protected void InteractableUpdate()
        {
            if (_moveCallback != null) _moveCallback();
        }

        private void OnDestroy()
        {
            items.Remove(this);
        }
    }
}
