using System.Collections;
using TMPro;
using UnityEngine;

namespace Arlo.UI
{
    /// <summary>
    /// The class for UI notifications.
    /// </summary>
    public class Notification : MonoBehaviour
    {
        /// <summary>
        /// Creates a notification and writes it.
        /// </summary>
        /// <param name="text">The text to display on the notification</param>
        public static void fabricate(string text)
        {
            var instance = new GameObject("notification");
            instance.transform.parent = GameObject.Find("Canvas").transform;
            
            Notification notif = instance.AddComponent<Notification>();
            notif.text = text;
            notif.write();
        }

        /// <summary>
        /// The text displayed on the notification.
        /// </summary>
        public string text;

        /// <summary>
        /// Display the notificatoin
        /// </summary>
        public void write()
        {
            StartCoroutine(writeAsync());
        }
        
        /// <summary>
        /// The private function used by the public <see cref="write"/> method to actually display the notification.
        /// </summary>
        private IEnumerator writeAsync()
        {
            // Set position of notificatoin.
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(1, 0);
            rectTransform.anchoredPosition = Vector2.zero;

            // Create text.
            var tmp = gameObject.AddComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 180;
            tmp.autoSizeTextContainer = true;
            tmp.text = text;

            yield return new WaitForSeconds(0.5f);

            /// Move the notification down until it is off teh screen, at which point delete it.
            var timeStart = Time.timeSinceLevelLoad;
            while (rectTransform.anchoredPosition.y > -rectTransform.sizeDelta.y)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, -rectTransform.sizeDelta.y), Mathf.Min(1, (Time.timeSinceLevelLoad - timeStart) / 2));
                tmp.color = new Color(255, 255, 255, 1 - (Time.timeSinceLevelLoad - timeStart) / 2);
                yield return new WaitForSeconds(0.01f);
            }
            GameObject.Destroy(this);
        } 
    }
}
