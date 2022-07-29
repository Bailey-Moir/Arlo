using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// The class for a selectable object in the world.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        // DEPENDENCIES

        /// <summary>
        /// The sprite the object uses its default state.
        /// </summary>
        [SerializeField]
        private Sprite _normalSprite;
        /// <summary>
        /// The sprite the object uses when it is highlighted.
        /// </summary>
        [SerializeField]
        private Sprite _highlightedSprite;

        /// <summary>
        /// The player in the scene.
        /// </summary>
        public GameObject player;

        // COMPONENTS

        /// <summary>
        /// The sprite render for the interactable object.
        /// </summary>
        private SpriteRenderer sr;
        /// <summary>
        /// The minimum distance the player must be away from the objec to interact with it.
        /// </summary>
        public float interactDistance;

        /// <summary>
        /// If the item is interactable.
        /// </summary>
        public bool interactable;

        private bool _highlighted = false;
        /// <summary>
        /// If the object is currently highlighted.
        /// </summary>
        public bool Highlighted
        {
            get => _highlighted;
            set
            {
                if (_highlighted != value)
                {
                    _highlighted = interactable ? value : false;
                    sr.sprite = _highlighted ? _highlightedSprite : _normalSprite;
                }
            }
        }

        /// <summary>
        /// The callback called when the item is interacted with.
        /// </summary>
        public System.Action interact = null;

        /// <summary>
        /// The update function to be used by any potentiaonl subclasses.
        /// </summary>
        protected virtual void InteractableUpdate() { }

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (interactable && Mathf.Abs((transform.position - player.transform.position).magnitude) <= interactDistance)
            {
                Highlighted = true;
                if (Input.GetKeyDown(KeyCode.E) && interact != null) interact();
            }
            else Highlighted = false;
            InteractableUpdate();
        }
    }
}
