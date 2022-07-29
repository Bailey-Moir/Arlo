using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arlo
{
    /// <summary>
    /// The class for the last scene of the game.
    /// </summary>
    public class BossBattleScene : MonoBehaviour
    {
        // DEPENDENCIES

        // The tree's mouth and eyes.
        /// <summary>
        /// One of the tree's components.
        /// </summary>
        public SpriteRenderer treeMouth, treeEyes;
        // The tree's sprites for the mouth and eyes.
        /// <summary>
        /// One of the sprites used to display the trees emotions.
        /// </summary>
        public Sprite normalEyes, squintEyes, sadOpenMouth, happyOpenMouth, happyMouth, sadMouth;
        /// <summary>
        /// The tree entity.
        /// </summary>
        public Entities.Attacker tree;
        /// <summary>
        /// The sprite for the stick projectile.
        /// </summary>
        public Sprite stickSprite;
        /// <summary>
        /// The player instance in the scene.
        /// </summary>
        public Entities.Player player;
        /// <summary>
        /// The end screen to be showed when the boss is beaten.
        /// </summary>
        public GameObject endPanel;

        void Start()
        {
            tree.Combat = false;
            tree.onHealthUpdate((float delta) => {
                if (tree.Health <= tree.MaxHealth / 2)
                {
                    // If the tree health is below half change expression.
                    treeMouth.sprite = sadMouth;
                    treeEyes.sprite = squintEyes;
                    // If the tree is dead, then start the end of the game.
                    if (tree.Dead) StartCoroutine(End());
                }
            });
        }

        /// <summary>
        /// The function used by the combat trigger that directs to <see cref="CombatCoroutine"/>
        /// </summary>
        public void CombatStart()
        {
            StartCoroutine(CombatCoroutine());
        }

        /// <summary>
        /// Called when the player touched the combat collider, and starts the dialog and combat.
        /// </summary>
        public IEnumerator CombatCoroutine()
        {
            // Makes the mouth move while the chat bubbles are showing up.
            StartCoroutine(TreeSpeak(10));
            yield return new ChatBubble("I see you, child", treeEyes.gameObject, 2).write();
            yield return new ChatBubble("there is much darkness within you", treeEyes.gameObject, 3).write();
            yield return new ChatBubble("do not fear little one,\nfor I will expell it", treeEyes.gameObject, 4).write();
            yield return new ChatBubble("come forth parasite", treeEyes.gameObject, 1).write();

            tree.Combat = true;
            player.Combat = true;

            // Turns the player into incarnate by changing the sprite colour of the player, increasing it's size, changing the colour of the health meter, and increasing the health, all using linear interpolation.
            var startFadeTime = Time.timeSinceLevelLoad;
            var playerSr = player.GetComponentInChildren<SpriteRenderer>();
            var healthMeterImage = Scene.ActiveScene.uiManager.healthMeterTransform.GetComponent<Image>();
            while (Time.timeSinceLevelLoad <= startFadeTime + 2)
            {
                var t = (Time.timeSinceLevelLoad - startFadeTime) / 2;
                playerSr.color = Color.Lerp(Color.white, Color.black, t);
                healthMeterImage.color = Color.Lerp(Color.red, new Color(0.294f, 0.294f, 0.294f, 1), t);
                player.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1.5f, 1), t);
                yield return new WaitForSeconds(0.1f);
            }
            playerSr.color = new Color(0, 0, 0, 1);
            healthMeterImage.color = new Color(0.294f, 0.294f, 0.294f, 1);
            player.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            player.MaxHealth = 50;
            player.Health = 50;
            tree.Combat = true;
            tree.healthBarFill.parent.Translate(new Vector3(0, -5, 0));

            // Start the combat loop.
            StartCoroutine(spikeLoop());
            StartCoroutine(shootLoop(5));
        }

        /// <summary>
        /// Called when the boss dies, ending the game.
        /// </summary>
        public IEnumerator End()
        {
            tree.Combat = false;
            player.Combat = false;

            // Make the mouth of the tree move during dialog.
            StartCoroutine(TreeSpeak(4));
            yield return new ChatBubble("I failed you...", treeEyes.gameObject, 4).write();
            yield return new WaitForSeconds(1);

            // Fade the tree.
            var timeStarted = Time.timeSinceLevelLoad;
            bool wait = true;
            tree.onUpdate(() =>
            {
                var t = (Time.timeSinceLevelLoad - timeStarted)/2;
                tree.sr.color = Color.white - Color.black * t;
                treeMouth.color = Color.white - Color.black * t;
                treeEyes.color = Color.white - Color.black * t;
                if (t >= 1)
                {
                    Destroy(tree.gameObject);
                    return true;
                }
                return false;
            });
            yield return new WaitUntil(() => wait);
            yield return new WaitForSeconds(3);

            // CHange to end screen.
            Scene.ActiveScene.uiManager.Panels.Add(endPanel);
            Scene.ActiveScene.uiManager.Panel = Scene.ActiveScene.uiManager.Panels.Count - 1;
            yield return new WaitForSeconds(3);
            // Close the game.
            Scene.ActiveScene.Quit();         

        }

        /// <summary>
        /// Animates the tree to make it look like it is speaking for <paramref name="time"/> amount of time.
        /// </summary>
        /// <param name="time">The amount of time to animate for</param>
        public IEnumerator TreeSpeak(float time)
        {
            var timeStart = Time.timeSinceLevelLoad;
            while (Time.timeSinceLevelLoad < timeStart + time)
            {
                treeMouth.sprite = tree.Health >= tree.MaxHealth/2 ? happyOpenMouth : sadOpenMouth;
                yield return new WaitForSeconds(0.2f);
                if (Time.timeSinceLevelLoad >= timeStart + time) break;
                treeMouth.sprite = tree.Health >= tree.MaxHealth / 2 ? happyMouth : sadMouth;
                yield return new WaitForSeconds(0.2f);
            }

            treeMouth.sprite = tree.Health >= tree.MaxHealth / 2 ? happyMouth : sadMouth;
        }

        /// <summary>
        /// Starts loops that shoots <paramref name="n"/> projectiles per cycle, producing 5 per second.
        /// </summary>
        /// <param name="n">The amoutn of projectiles to shoot.</param>
        public IEnumerator shootLoop(int n)
        {
            while (!tree.Dead)
            {
                var timeStart = Time.timeSinceLevelLoad;

                // The number of projectiles that are currently alive
                int count = 0;
                for (count = 0; count <= n; count++)
                {
                    // Create the projectile, and decrease 'count' when it is deleted.
                    Entities.TrackingProjectile.fabricate(treeMouth.gameObject, player, stickSprite, 3, 12, -1, 200, 6).onDeletion = () => count--;
                    yield return new WaitForSeconds(0.2f);
                }

                yield return new WaitUntil(() => count == 0);
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Starts loop that creates spikes every 4 seconds.
        /// </summary>
        public IEnumerator spikeLoop()
        {
            var playerSr = player.GetComponentInChildren<SpriteRenderer>();
            while (!tree.Dead)
            {
                // Creats the spike and starts the attack.
                StartCoroutine(Entities.GroundSpike.fabricate(player, 2, playerSr.sortingOrder - 1).attack());
                yield return new WaitForSeconds(4);
            }
        }
    }
}
