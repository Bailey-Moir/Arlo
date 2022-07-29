using System.Collections;
using UnityEngine;
using Arlo.Entities;

namespace Arlo.Entities
{
    /// <summary>
    /// The script that controls the player.
    /// </summary>
    public class Player : Entity
    {
        /// <summary>
        /// The last direction the player moved in. Used to determine the direction of attack if the player attacks using keyboard and not mouse.
        /// </summary>
        private Vector3 _lastDirection;

        /// <summary>
        /// The last time that the player attacked. Used to determine if the player can attack again to tell the UI how long until he can again.
        /// </summary>
        public float lastAttack = -0.5f;

        // DEPENDENCIES

        /// <summary>
        /// The transform of the object with the player sprite, allowing the sprite to be offsetted without moving the camera, collider, or rigid body.
        /// </summary>
        private Transform _spriteOffset;

        // COMPONENTS

        /// <summary>
        /// The player animator component.
        /// </summary>
        private Animator _anim;
        
        private Rigidbody2D _rb;
        /// <summary>
        /// The players rigidbody component.
        /// </summary>
        public Rigidbody2D Rb { get => _rb; }

        protected override void StackableAwake()
        {
            base.StackableAwake();

            _rb = GetComponent<Rigidbody2D>();
            _spriteOffset = transform.GetChild(0);
            _anim = _spriteOffset.GetComponent<Animator>();
            sr = _spriteOffset.GetComponent<SpriteRenderer>();
            Col = GetComponent<Collider2D>();
        }
        
        private void Start() {
            Combat = Scene.ActiveScene.defaultCombatState;
        }

        protected override void StackableUpdate()
        {
            base.StackableUpdate();

            // If clicks space during combat, attack.
            if ((Settings.useMouse ? Input.GetButtonDown("Fire2") : Input.GetKeyDown(KeyCode.Space)) && Time.timeSinceLevelLoad - lastAttack > 0.5f && Combat) StartCoroutine(Attack());

            if (Input.GetKeyDown(KeyCode.Escape)) Scene.ActiveScene.uiManager.Panel = Scene.ActiveScene.uiManager.Panel == -1 ? 0 : -1;
        }

        void FixedUpdate()
        {
            Vector3 translateVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

            RaycastHit2D collisionTest = Physics2D.Raycast(transform.position, translateVector, speed * Time.deltaTime);

            if ( !collisionTest || collisionTest.transform.gameObject == this.gameObject || collisionTest.collider.isTrigger || collisionTest.transform.gameObject.layer == 6) _rb.MovePosition(transform.position + translateVector * speed * Time.deltaTime);
            else translateVector = Vector3.zero;

            if (translateVector != Vector3.zero) this._lastDirection = translateVector;

            // Animation
            _anim.SetBool("walking", translateVector.x != 0 || translateVector.y != 0);
            _anim.SetBool("left", translateVector.x < 0);
            _anim.SetBool("right", translateVector.x > 0);
            _anim.SetBool("down", translateVector.y < 0);
            _anim.SetBool("up", translateVector.y > 0);
        }

        /// <summary>
        /// Aysnchronous function that is used to make the player attack.
        /// </summary>
        private IEnumerator Attack()
        {
            lastAttack = Time.timeSinceLevelLoad;
            
            Vector3 delta = Vector3.zero;
            if (Settings.useMouse)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)
                );
                Vector3 diff = mousePos - transform.position;
                float angle = Mathf.Atan(diff.y / diff.x);

                float multiplier = diff.x >= 0 ? 0.5f : -0.5f;

                delta = new Vector3(Mathf.Cos(angle) * multiplier, Mathf.Sin(angle) * multiplier, 0);
            }
            else
            {
                delta = Approach(transform.position, _lastDirection + transform.position, 1);
            }

            _spriteOffset.Translate(delta);

            var attackersArray = Attacker.Attackers.ToArray();
            foreach (var attacker in attackersArray) if (attacker && Mathf.Abs((transform.position - attacker.transform.position).magnitude) <= 1) attacker.Health -= dmg;
            Attacker.Attackers.Clear();
            Attacker.Attackers.AddRange(attackersArray);

            yield return new WaitForSeconds(0.1f);

            _spriteOffset.Translate(-delta);
        }
    }
}