using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Arlo.UI
{
    /// <summary>
    /// The class for the component used in the UI container to manager panels.
    /// </summary>
    public class Manager : MonoBehaviour
    {
        // DEPENDECIES

        /// <summary>
        /// The player instance in the scene.
        /// </summary>
        public Entities.Player player;

        /// <summary>
        /// The rect transform class of the health meter.
        /// </summary>
        public RectTransform healthMeterTransform;

        /// <summary>
        /// All the panels in the UI.
        /// </summary>
        public List<GameObject> Panels = new List<GameObject>();

        /// <summary>
        /// The UI that shows the player how far into their attack cooldown they are.
        /// </summary>
        public RectTransform attackDelay;

        /// <summary>
        /// The UI setting that tells uses <see cref="changeDirector(bool)"/>.
        /// </summary>
        public Toggle useMouse;

        // PROPERTIES

        private int _panel = -1;
        /// <summary>
        /// The current panel being dispalyed in the UI. If -1, nothing is displayed.
        /// </summary>
        public int Panel
        {
            get => _panel;
            set
            {
                if (_panel != value)
                {
                    if (_panel != -1) Panels[_panel].SetActive(false);
                    else Time.timeScale = 0;
                    if (value != -1) Panels[value].SetActive(true);
                    else Time.timeScale = 1;
                    _panel = value;
                }
            }
        }

        void Awake()
        {
            useMouse.isOn = Settings.useMouse;
            player = GameObject.Find("Arlo").GetComponent<Entities.Player>();

            player.onHealthUpdate((float _) => {
                healthMeterTransform.sizeDelta = new Vector2(240 * player.Health / player.MaxHealth, 15.02f);
                healthMeterTransform.localPosition = new Vector2(120 - 120 * player.Health / player.MaxHealth, 0);

                if (player.Health <= 0) Scene.ActiveScene.Restart();
            });

            player.onCombatChange((bool entering) => healthMeterTransform.transform.parent.gameObject.SetActive(entering));
        }

        void Update()
        {
            attackDelay.sizeDelta = new Vector2(125 * (1 - 2*Mathf.Min(0.5f,Time.timeSinceLevelLoad - player.lastAttack)), 0);
            attackDelay.localPosition = new Vector2(62.5f*2*Mathf.Min(0.5f, Time.timeSinceLevelLoad - player.lastAttack), 0);
        }

        /// <summary>
        /// Used by UI to change if the player uses right click to direct attacks to the cursor, or space to direct attacks in the direction last facing.
        /// </summary>
        /// <param name="b">If the player uses the mouse.</param>
        public void changeDirector(bool b) => Settings.useMouse = b;
    }
}
