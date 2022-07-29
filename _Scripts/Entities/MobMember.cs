using UnityEngine;

namespace Arlo.Entities
{
    /// <summary>
    /// The class that represents each mob member
    /// </summary>
    public class MobMember : Attacker
    {
        /// <summary>
        /// The way that the mob member is attacking right now.
        /// </summary>
        public enum CombatState
        {
            Chasing,
            Fleeing,
            Preparing
        }

        /// <summary>
        /// The time the AI started the current combat state.
        /// </summary>
        private float _timeStarted;
        /// <summary>
        /// The direction that the AI is preparing to attack towards (if preparing).
        /// </summary>
        private Vector2 _preparingDirection;
        /// <summary>
        /// When the AI started preparing to attack.
        /// </summary>
        private Vector2 _preparingStart;

        private CombatState _state;
        /// <summary>
        /// The AI's current combat state.
        /// </summary>
        public CombatState State { get => _state; }

        // COMPONENTS

        /// <summary>
        /// The AI's animatior component.
        /// </summary>
        private Animator _anim;

        protected override void StackableAwake()
        {
            base.StackableAwake();

            Rb = GetComponent<Rigidbody2D>();
            Col = GetComponent<Collider2D>();
            _anim = GetComponent<Animator>();

            onHealthUpdate((float delta) => { 
                if (Dead) Destroy(gameObject);
            });
        }

        public void FixedUpdate()
        {
            if (!holt)
            {
                Vector3 movement = Combat ? (
                        State == CombatState.Chasing
                            ? Approach(transform.position, attackee.transform.position, speed) * Time.fixedDeltaTime
                        : State == CombatState.Fleeing
                            ? Disperse(transform.position, attackee.transform.position, speed) * Time.fixedDeltaTime
                        : (Vector3)(_preparingStart - _preparingDirection * speed / 3 * (3 * Mathf.Pow(Time.timeSinceLevelLoad - _timeStarted, 2) - Time.timeSinceLevelLoad + _timeStarted)) - transform.position
                    ) : Vector3.zero;

                // Checks if the vector is NaN (if one dimension, is they all are).
                if (float.IsNaN(movement.x)) movement = Vector3.zero;

                if (Combat)
                {
                    Rb.MovePosition(movement + transform.position);

                    if (_state == CombatState.Chasing && Mathf.Abs((transform.position - attackee.transform.position).magnitude) <= 0.5)
                    {
                        // Reset time counter
                        _timeStarted = Time.timeSinceLevelLoad;

                        _preparingDirection = Disperse(transform.position, attackee.transform.position, speed);
                        _preparingStart = transform.position;

                        // Change state
                        _state = CombatState.Preparing;
                        foreach (var Attacker in Attackers) if (Attacker != this) Physics2D.IgnoreCollision(Col, Attacker.Col);
                    }
                    else if (_state == CombatState.Fleeing)
                    {
                        // Fleeing
                        var collisionTest = Physics2D.Raycast(transform.position, movement, -Time.fixedDeltaTime);

                        if (Time.timeSinceLevelLoad - _timeStarted >= 4.5 / speed || (collisionTest && collisionTest.transform.gameObject != attackee.gameObject && collisionTest.transform.gameObject.layer != 6)) _state = CombatState.Chasing;
                    }
                    else if (_state == CombatState.Preparing && Time.timeSinceLevelLoad - _timeStarted >= 0.333333)
                    {
                        foreach (var Attacker in Attackers) if (Attacker != this) Physics2D.IgnoreCollision(Col, Attacker.Col, false);
                        if (Mathf.Abs((transform.position - attackee.transform.position).magnitude) <= 1)
                        {
                            // Reset time counter
                            _timeStarted = Time.timeSinceLevelLoad;
                            // Moves the AI to the player
                            transform.position = attackee.transform.position;
                            // Makes the AI flee
                            _state = CombatState.Fleeing;
                            // Make player looses damage
                            attackee.Health -= dmg;
                        }
                        else _state = CombatState.Chasing;
                    }

                    var walking = movement.x != 0 || movement.y != 0;

                    if (walking) _anim.speed = this.speed / 6;

                    _anim.SetBool("walking", walking);
                    _anim.SetBool("left", movement.x <= movement.y && movement.y <= -movement.x);
                    _anim.SetBool("up", Mathf.Abs(movement.x) < movement.y);
                    _anim.SetBool("right", -movement.x <= movement.y && movement.y <= movement.x);
                    _anim.SetBool("down", movement.x > movement.y && -movement.x > movement.y);
                }
            }
            else
            {
                _anim.SetBool("walking", false);
                _anim.SetBool("left", true);
                _anim.SetBool("up", false);
                _anim.SetBool("right", true);
                _anim.SetBool("down", false);
            }
        }
    }
}
