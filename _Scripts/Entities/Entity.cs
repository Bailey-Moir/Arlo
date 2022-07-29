using System.Collections.Generic;
using UnityEngine;

namespace Arlo.Entities
{
    /// <summary>
    /// The class that represents each entity in the game.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        /// <summary>
        /// Creates a vector to move by inorder to move from the current position towards (not to) the given point.
        /// </summary>
        /// <param name="current">The current position that you want to move from.</param>
        /// <param name="point">The point that you want to move towards.</param>
        /// <param name="distance">The distance to move towards it</param>
        /// <returns>The vector to add to the current position to make it move closer to the given point.</returns>
        static protected Vector3 Approach(Vector3 current, Vector3 point, float distance)
        {
            Vector3 direction = current - point;
            float angle = Mathf.Atan(direction.y / direction.x);

            int side = direction.x >= 0 ? -1 : 1;

            return new Vector3(side * Mathf.Cos(angle) * distance, side * Mathf.Sin(angle) * distance, 0);
        }
        /// <summary>
        /// Creates a vector to move by inorder to move from the current position away from the given point.
        /// </summary>
        /// <param name="current">The current position that you are at.</param>
        /// <param name="origin">The original point that you want to move away from.</param>
        /// <returns>The vector to add to the current position to make it move further away from the given point.</returns>
        static protected Vector3 Disperse(Vector3 current, Vector3 origin, float distance)
        {
            Vector3 direction = origin - current;
            float angle = Mathf.Atan(direction.y / direction.x);

            if (float.IsNaN(angle)) angle = Random.Range(-90, 90);

            int side = direction.x != 0 ? (direction.x > 0 ? 1 : -1) : (Random.Range(-1, 1) >= 0 ? 1 : -1);

            return -new Vector3(side * Mathf.Cos(angle) * distance, side * Mathf.Sin(angle) * distance, 0);
        }

        /// <summary>
        /// The transform of the health bar (may not exist).
        /// </summary>
        public Transform healthBarFill = null;

        /// <summary>
        /// Move the entity to a point linearly.
        /// </summary>
        /// <param name="point">The local point</param>
        public void linearlyGo(Vector3 point)
        {
            var time = Time.timeSinceLevelLoad;
            var origin = transform.localPosition;
            updateCallbacks.Add(() => {
                var t = (Time.timeSinceLevelLoad - time) / (Mathf.Abs((point - origin).magnitude) / speed);
                transform.localPosition = Vector2.Lerp(origin, point, t);
                if (t > 1)
                {
                    transform.localPosition = point;
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Move the entity from one point to another linearly, and call a function upon completion
        /// </summary>
        /// <param name="point">The local point</param>
        /// <param name="onCompletion">The callback to be called upon completion</param>
        public void linearlyGo(Vector3 point, System.Action onCompletion)
        {
            var time = Time.timeSinceLevelLoad;
            var origin = transform.localPosition;
            updateCallbacks.Add(() => {
                var t = (Time.timeSinceLevelLoad - time) / (Mathf.Abs((point - origin).magnitude) / speed);
                transform.localPosition = Vector2.Lerp(origin, point, t);
                if (t > 1)
                {
                    transform.localPosition = point;
                    onCompletion();
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// If the Entity is dead or not.
        /// </summary>
        public bool Dead {
            set { if (value) Health = 0; }
            get => Health <= 0; 
        }

        /// <summary>
        /// The amount of damage the entity does.
        /// </summary>
        protected float dmg = 4;

        /// <summary>
        /// The SpriteRenderer of the entity itself.
        /// </summary>
        public SpriteRenderer sr = null;

        /// <summary>
        /// Whether to display the default entity health bar.
        /// </summary>
        [SerializeField]
        protected bool displayBar = true;

        /// <summary>
        /// The speed at which the entity moves.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// If the entity should stop attacking/moving/doing stuff, e.g. because it just got hit.
        /// </summary>
        public bool holt = false;

        /// <summary>
        /// The add to global time to make local entity time, accounts for when the entity was created, and holted.
        /// </summary>
        protected float entityTimeOffset = 0;

        // Callbacks
        /// <summary>
        /// The list of callbacks that are called whenver the combat state is changed.
        /// </summary>
        private List<System.Action<bool>> _combatCallbacks = new List<System.Action<bool>>();
        /// <summary>
        /// The list of callbacks that are called whenver the health of the entity is changed.
        /// </summary>
        private List<System.Action<float>> _healthCallbacks = new List<System.Action<float>>();
        /// <summary>
        /// The callback for the entities current 'LinearlyGo' callback if it exists, amoung others that are called on update.
        /// </summary>
        private List<System.Func<bool>> updateCallbacks = new List<System.Func<bool>>();

        /// <summary>
        /// The entities version of the 'Awake' method.
        /// </summary>
        protected virtual void StackableAwake() {
            this.entityTimeOffset -= Time.timeSinceLevelLoad;
            TryGetComponent<SpriteRenderer>(out sr);
            this.Health = MaxHealth;
        }

        /// <summary>
        /// The entities version of the 'Update' method.
        /// </summary>
        protected virtual void StackableUpdate() {
            // Removes all callbacks that return true; all the callbacks that are done.
            updateCallbacks.RemoveAll(c => c());
        }

        private Collider2D _col;
        /// <summary>
        /// The entity's collider.
        /// </summary>
        public Collider2D Col {  get => _col; protected set => _col = value; }

        [SerializeField]
        private float _maxHealth = 25;
        /// <summary>
        /// The maximum health that the entity can have.
        /// </summary>
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        private float _health = 0;
        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public float Health { get => _health; set {
            float oldHealth = _health;

            _health = value >= MaxHealth ? MaxHealth : value;

            // Change health bar approriately.
            if (displayBar && Combat)
            {
                healthBarFill.localScale = new Vector3(7f / 8f * _health / MaxHealth, 3f / 16f, 0);
                healthBarFill.localPosition = new Vector3((1 - _health/ MaxHealth) * 7f / -16f, 0, 0);
            }

            _healthCallbacks.ForEach((System.Action<float> c) => c.Invoke(-_health - oldHealth));
        }}

        /// <summary>
        /// The private variable of if the entity is in combat. Most of the time should not be used, in favor of '<see cref="Combat">Combat</see>'.
        /// </summary>
        private bool _combat = false;
        /// <summary>
        /// Whether the entity is current in combat.
        /// </summary>
        public bool Combat
        {
            get => _combat;
            set
            {
                if (value == _combat) return;

                _combat = value;

                _combatCallbacks.ForEach((System.Action<bool> c) => c.Invoke(value));

                if (!displayBar) return;

                // Create of delete health bar depending on if it already exists.
                if (value)
                {
                    GameObject healthBar = GameObject.Instantiate(GlobalStorage.healthBarPrefab);
                    healthBar.transform.parent = transform;

                    healthBar.GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;

                    healthBar.transform.localPosition = new Vector3(0, (sr.sprite.rect.height + 2f) / 16, 0);

                    healthBarFill = healthBar.transform.GetChild(0);
                    healthBarFill.GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 2;

                    healthBarFill.localScale = new Vector3(7f / 8f * Health / MaxHealth, 3f / 16f, 0);
                    healthBarFill.localPosition = new Vector3((1 - Health / MaxHealth) * 7f / -16f, 0, 0);
                }
                else GameObject.Destroy(healthBarFill.parent.gameObject);
            }
        }

        private void Update()
        {
            StackableUpdate();
        }

        void Awake()
        {
            StackableAwake();
        }

        /// <summary>
        /// Add a callback to be run whenever entering or leaving combat.
        /// </summary>
        /// <param name="c">The callback to add.</param>
        public void onCombatChange(System.Action<bool> c) => _combatCallbacks.Add(c);
        /// <summary>
        /// Add a callback to be run whenever entering the health of the entity is updated.
        /// </summary>
        /// <param name="c">The callback to add.</param>
        public void onHealthUpdate(System.Action<float> c) => _healthCallbacks.Add(c);

        /// <summary>
        /// Add a callback to be run on update that returns whether or not it should stop running or not.
        /// </summary>
        /// <param name="c">The callback to be run that returns whether or not it should be stopped in future.</param>
        public void onUpdate(System.Func<bool> c) => updateCallbacks.Add(c);
    }
}