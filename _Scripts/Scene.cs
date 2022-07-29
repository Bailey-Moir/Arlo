using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arlo
{
    /// <summary>
    /// All scenes have this script. Has the bounding box of the camera and cause the ignoring of necessary collisions.
    /// </summary>
    public class Scene : MonoBehaviour
    {
        /// <summary>
        /// The scene that is currently active. Do not using during or before 'Awake()' function.
        /// </summary>
        public static Scene ActiveScene { get => _activeScene; private set => _activeScene = value; }
        private static Scene _activeScene;

        /// <summary>
        /// The default combat state of the player (in combat or out of combat).
        /// </summary>
        public bool defaultCombatState;

        /// <summary>
        /// The UI manager.
        /// </summary>
        public UI.Manager uiManager;

        void Awake() {
            ActiveScene = this;
            uiManager = GetComponent<UI.Manager>();
            
            Physics2D.IgnoreLayerCollision(6, 3); // Ignore entity - environment collisons.
            Physics2D.IgnoreLayerCollision(6, 8); // Ignore entity - obstruction collisons.
            Physics2D.IgnoreLayerCollision(6, 7); // Ignore entity - player collisons
        }

        /// <summary>
        /// To be used in something like an <see cref="EventTrigger">EventTrigger</see> to set the scene.
        /// </summary>
        /// <param name="n">The scene number.</param>
        public void Load(int n)
        {
            SceneManager.LoadScene(n, LoadSceneMode.Single);
        }

        /// <summary>
        /// Restarts the level. To be used in the menu.
        /// </summary>
        public void Restart()
        {
            uiManager.Panel = -1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        /// <summary>
        /// Closes the game.
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }
}
