using System.Collections;
using UnityEngine;
using Arlo.Entities;

namespace Arlo
{
    /// <summary>
    /// The class for the first scene of the game.
    /// </summary>
    public class LandInvasionScene : MonoBehaviour
    {
        // DEPEDENCIES

        /// <summary>
        /// The centre Mob Member.
        /// </summary>
        public MobMember madGuy;
        /// <summary>
        /// The left Mob Member.
        /// </summary>
        public MobMember leftGuy;
        /// <summary>
        /// The right Mob Member.
        /// </summary>
        public MobMember rightGuy;
        /// <summary>
        /// The player instance.
        /// </summary>
        public Player player;
        /// <summary>
        /// The gate that stops the player from continuing to the house during combat.
        /// </summary>
        public GameObject exitGate;
        /// <summary>
        /// The gate that stops the player from going back where they came from during combat.
        /// </summary>
        public GameObject entryGate;
        public Incarnate incarnate;

        /// <summary>
        /// If the tutorial has started.
        /// </summary>
        private bool tutorialStarted = false;
        /// <summary>
        /// If the tutorial has finished.
        /// </summary>
        private bool tutorialFinished = false;

        /// <summary>
        /// The function used by the 'Combat Trigger' to trigger the animaton of the mob members and the enabling of combat.
        /// </summary>
        public void CombatStart()
        {
            incarnate.gameObject.SetActive(false);

            tutorialFinished = true;

            // Stops the player from going back
            entryGate.SetActive(true);
            exitGate.SetActive(true);

            // Creates the chat bubble
            StartCoroutine((new ChatBubble("Get off our land", madGuy.gameObject, 3)).write());

            // Adds the callback to each
            foreach (MobMember member in new MobMember[] { madGuy, leftGuy, rightGuy }) member.onHealthUpdate((float delta) => {
                if (madGuy.Dead && leftGuy.Dead && rightGuy.Dead)
                {
                    incarnate.transform.position = new Vector3(8, -0.5f, 0);
                    incarnate.gameObject.SetActive(true);

                    var bubble = new ChatBubble("Good job.", incarnate.gameObject, 0.75f);
                    bubble.onDecay = () => incarnate.linearlyGo(new Vector3(incarnate.transform.position.x, 20), () => Destroy(incarnate.gameObject));
                    StartCoroutine(bubble.write());

                    Destroy(entryGate);
                    Destroy(exitGate);

                    player.Combat = false;
                }
            });

            leftGuy.linearlyGo(new Vector2(leftGuy.transform.position.x, madGuy.transform.position.y));
            rightGuy.linearlyGo(new Vector2(rightGuy.transform.position.x, madGuy.transform.position.y), () => {
                leftGuy.Combat = true;
                rightGuy.Combat = true;
                madGuy.Combat = true;
                player.Combat = true;
            });
        }

        private IEnumerator StartTutorial()
        {
            yield return new WaitForSeconds(1);
            yield return new ChatBubble("Hello there Father.", incarnate.gameObject, 2.5f).write();
            yield return new ChatBubble("Use W A S D or the\narrow keys to move", incarnate.gameObject, 4f).write();
            // Stop if the player has skipped tutorial.
            if (!incarnate.gameObject.activeSelf) yield break;
            yield return new ChatBubble("Use the mouse to aim and right\nclick to attack in combat", incarnate.gameObject, 4f).write();
            // Stop if the player has skipped tutorial.
            if (!incarnate.gameObject.activeSelf) yield break;
            yield return new ChatBubble("Now wreak havoc!", incarnate.gameObject, 3f).write();
        }

        void Update()
        {
            if (!tutorialStarted && Input.anyKey)
            {
                tutorialStarted = true;
                incarnate.gameObject.SetActive(true);
                StartCoroutine(StartTutorial());
            }

            if (tutorialStarted && !tutorialFinished)
            {
                incarnate.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 3, player.transform.position.z);
            }
        }
    }
}