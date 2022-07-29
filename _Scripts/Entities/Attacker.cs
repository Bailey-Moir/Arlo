using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arlo.Entities
{
    /// <summary>
    /// The very abstract class for an AI for an Entity that attacks the player.
    /// </summary>
    public class Attacker : Entity
    {
        public static List<Attacker> Attackers = new List<Attacker>();

        /// <summary>
        /// The entity that the attacker is attacking
        /// </summary>
        public Entity attackee;

        /// <summary>
        /// The particle system used by the attacker to take damage.
        /// </summary>
        public ParticleSystem hitPS;

        // The attacker's rigidbody component.
        private Rigidbody2D _rb;
        /// <summary>
        /// The attacker's rigidbody component.
        /// </summary>
        public Rigidbody2D Rb { get => _rb; protected set => _rb = value; }
        
        protected override void StackableAwake()
        {
            base.StackableAwake();
            onHealthUpdate((float delta) =>
            {
                // Run take hit animatoin if the attacker lost health.
                if (delta < 0 && Combat) StartCoroutine(takeHit());
            });
        }

        private void OnEnable()
        {
            Attackers.Add(this);
        }

        private void OnDestroy()
        {
            Attackers.Remove(this);
        }
        
        /// <summary>
        /// The function called when the entity is hit, runs animations and the particle system.
        /// </summary>
        protected IEnumerator takeHit()
        {
            holt = true;
            var originalPosition = transform.position;
            var startTime = Time.timeSinceLevelLoad;

            // Player particle effect.
            hitPS.transform.position = transform.position + Vector3.up*0.75f;
            hitPS.Play();

            // Move around randommly.
            for (int i = 1; i <= 6; i++)
            {
                transform.position = originalPosition + Disperse(originalPosition, originalPosition, 0.1f);
                yield return new WaitForSeconds(0.01f);
            }

            transform.position = originalPosition;
            holt = false;
            entityTimeOffset -= Time.timeSinceLevelLoad - startTime;
        }
    }
}
