using UnityEngine;

namespace Arlo.Entities
{
    public class TrackingProjectile : Attacker
    {
        /// <summary>
        /// Creates a new tracking projectile.
        /// </summary>
        /// <param name="shooter">The object that is shooting the object.</param>
        /// <param name="target">The object the projectile is tracking.</param>
        /// <param name="sprite">The sprite of the projectile.</param>
        /// <param name="dmg">The damage the projectile does.</param>
        /// <param name="speed">The speed of the projectile (us^-1).</param>
        /// <param name="sortingLayer">The sorting layer of the projectile.</param>
        /// <param name="bend">The amount that the projectile can bend per second.</param>
        /// <param name="life">The amoutn of time it takes for the projectile to delete itself.</param>
        /// <returns>The resultant tracking projectile.</returns>
        public static TrackingProjectile fabricate(GameObject shooter, Entity target, Sprite sprite, float dmg, float speed, int sortingLayer, float bend, float life)
        {
            GameObject instance = new GameObject("bullet");
            instance.transform.parent = shooter.transform;
            SpriteRenderer projectileSr = instance.AddComponent<SpriteRenderer>();
            projectileSr.sortingOrder = sortingLayer;
            projectileSr.sprite = sprite;
            TrackingProjectile script = instance.AddComponent<TrackingProjectile>();
            script.attackee = target;
            script.speed = speed;
            script.bend = bend;
            script.life = life;
            script.dmg = dmg;
            Vector3 difference = script.attackee.transform.position - instance.transform.position;
            instance.transform.rotation = Quaternion.Euler(0, 0, ((difference.x >= 0 ? (difference.y < 0 ? 2 : 0) : 1) * Mathf.PI + Mathf.Atan(difference.y / difference.x)) * 180 / Mathf.PI);

            return script;
        }

        /// <summary>
        /// The amount that the projectile can bend per second.
        /// </summary>
        public float bend;
        /// <summary>
        /// The amount of time the projectile takes before it deletes itself if it hasn't hit the <see cref="target"/>.
        /// </summary>
        public float life;
        /// <summary>
        /// Called when the projectile hits the <see cref="target"/>, or it reaches the end of it's <see cref="life"/>.
        /// </summary>
        public System.Action onDeletion = null;

        protected override void StackableAwake()
        {
            base.StackableAwake();
            Health = 1;
        }

        void FixedUpdate()
        {
            Vector3 difference = attackee.transform.position + Vector3.up * 0.5f - transform.position;

            // Set the rotation of the tracking projectile, only allowing it to turn a max of 'bend'.
            if (Mathf.Abs(Mathf.Atan(difference.y / difference.x) * 180 / Mathf.PI + transform.rotation.eulerAngles.z) < 45 || Mathf.Abs(((Vector2) difference).magnitude) > 360*speed/Mathf.PI/bend) transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, transform.rotation.z + ((difference.x >= 0 ? (difference.y < 0 ? 2 : 0) : 1) * Mathf.PI + Mathf.Atan(difference.y / difference.x))*180/Mathf.PI), bend * Time.deltaTime);
            transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        }

        // TODO: Fix Collisons
        protected override void StackableUpdate()
        {
            base.StackableUpdate();
            // If the projectile is close enough to the enemy, attack it and destroy itself.
            if (Mathf.Abs((attackee.transform.position + Vector3.up * 0.5f - transform.position).magnitude) <= 0.75f)
            {
                attackee.Health -= 5;
                Destroy(this.gameObject);
            }
                
            // If the projectile has been alive too long, destroy itself.
            if (Time.timeSinceLevelLoad + entityTimeOffset >= life) Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            if (onDeletion != null) onDeletion();
        }
    }
}
