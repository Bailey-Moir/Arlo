using UnityEngine;
using UnityEngine.Events;

namespace Arlo
{
    /// <summary>
    /// The class used for when you want an event to trigger when an object enters the current object's collider.
    /// </summary>
    public class EventTrigger : MonoBehaviour
    {
        /// <summary>
        /// The object that can enter this (the gameobject that has this script).
        /// </summary>
        public GameObject obj;

        /// <summary>
        /// If the object should delete itself after being triggered
        /// </summary>
        public bool deleteSelf;

        /// <summary>
        /// If the event trigger is currently triggerable;
        /// </summary>
        public bool triggerable = true;

        /// <summary>
        /// The event that triggers all events to happen when the <see cref="obj">given object</see> enters the event triggers collider.
        /// </summary>
        public UnityEvent onEnter = new UnityEvent();

        void OnTriggerEnter2D(Collider2D col) { if (col.gameObject == obj && triggerable) {
            onEnter.Invoke();
            if (deleteSelf) Destroy(this.gameObject);
        }}
    }
}