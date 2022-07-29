using UnityEngine;

namespace Arlo.Entities
{
    /// <summary>
    /// The script that controls the incarnate.
    /// </summary>
    public class Incarnate : Entity
    {
        /// <summary>
        /// The posiiton that the incarnate was at last fixed update.
        /// </summary>
        private Vector3 _lastPosition;

        // COMPONENTS

        /// <summary>
        /// The incarnate animator component.
        /// </summary>
        private Animator _anim;

        protected override void StackableAwake()
        {
            base.StackableAwake();

            _lastPosition = transform.position;

            _anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            Combat = false;

            _lastPosition = transform.position;
        }

        protected override void StackableUpdate()
        {
            base.StackableUpdate();

            // Direction incarnate is moving.
            Vector2 direction = transform.position - _lastPosition;
            _lastPosition = transform.position;

            if (direction == Vector2.zero) return;

            // Animation
            _anim.SetBool("walking", direction.x != 0 || direction.y != 0);
            _anim.SetBool("left", direction.x < 0);
            _anim.SetBool("right", direction.x > 0);
            _anim.SetBool("down", direction.y < 0);
            _anim.SetBool("up", direction.y > 0);
        }
    }
}