using System.Collections;
using TMPro;
using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// The class that represents a chat bubble.
    /// </summary>
    public class ChatBubble
    {
        /// <summary>
        /// The text on the chat bubble.
        /// </summary>
        public string text;
        /// <summary>
        /// How long after writing the chat bubble will dissapear.
        /// </summary>
        public float decay;
        /// <summary>
        /// The object that the chat bubble is visually emitting from.
        /// </summary>
        public GameObject source;

        /// <summary>
        /// The game object of the chat bubble that may or may not exist.
        /// </summary>
        private GameObject instance;

        /// <summary>
        /// Called when the chat bubble is destroyed.
        /// </summary>
        public System.Action onDecay = () => {};

        /// <summary>
        /// The contructor for the class.
        /// </summary>
        /// <param name="text">The text on the chat bubble.</param>
        /// <param name="source">The object that the chat bubble is visually emitting from.</param>
        /// <param name="decay">How long after writing the chat bubble will dissapear.</param>
        public ChatBubble(string text, GameObject source, float decay)
        {
            this.text = text;
            this.source = source;
            this.decay = decay;
        }

        /// <summary>
        /// Calculates the size that the sprite will be /is.
        /// </summary>
        /// <returns>The size of the sprite.</returns>
        private Vector2 calcSpriteSize()
        {
            // List of lines to be displayed.
            var lines = text.Split('\n');
            
            // The character width of the sprite, calculated by getting the largest character count of all of the lines.
            int charWidth = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > charWidth) charWidth = lines[i].Length;
            }

            int lineCount = lines.Length;
            // set the width to charWidth x 3.8 (a constant), but making sure that it stays uneven, and sets the height to the line count x 8 + 4 (both numbers random constants determined from testing).
            return new Vector2(((int)(charWidth * 3.8f) % 2 == 0 ? (int)(charWidth * 3.8f) + 1 : (int)(charWidth * 3.8f)) + 2, lineCount * 8 + 4);
        }

        /// <summary>
        /// Dynamically generates the sprite for the chat bubble.
        /// </summary>
        /// <returns>The sprite for the chat bubble</returns>
        private Sprite generateSprite()
        {
            Vector2 size = calcSpriteSize();

            Texture2D texture = new Texture2D((int)size.x, (int)size.y);

            // Bottom line
            for (int i = 0; i < (int)size.x; i++) texture.SetPixel(i, 0, i == ((int)size.x - 1) / 2 ? Color.black : Color.clear);

            // Second to bottom line
            for (int i = 0; i < (int)size.x; i++) texture.SetPixel(i, 1, i >= ((int)size.x - 1) / 2 - 1 && i <= ((int)size.x - 1) / 2 + 1 ?
               (i == ((int)size.x - 1) / 2 ? Color.white : Color.black)
                   :
               Color.clear
           );

            // Third to bottom line
            for (int i = 0; i < (int)size.x; i++) texture.SetPixel(i, 2, (i > 0 && i < ((int)size.x - 3) / 2) || (i >= ((int)size.x + 3) / 2 && i < (int)size.x - 1) ?
               Color.black
               :
               (i == 0 || i == (int)size.x - 1 ? Color.clear : Color.white)
           );

            // Middle lines
            for (int i = 3; i < (int)size.y - 1; i++)
            {
                for (int j = 0; j < (int)size.x; j++) texture.SetPixel(j, i, j == 0 || j == (int)size.x - 1 ? Color.black : Color.white);
            }

            // Top line
            for (int i = 0; i < (int)size.x; i++) texture.SetPixel(i, (int)size.y - 1, i == 0 || i == (int)size.x - 1 ? Color.clear : Color.black);

            texture.filterMode = FilterMode.Point;

            texture.Apply();

            Sprite result = Sprite.Create(
                texture,
                new Rect(0, 0, (int)size.x, (int)size.y),
                new Vector2(0.5f, 0f),
                16,
                1,
                SpriteMeshType.Tight,
                Vector4.zero
            );

            return result;
        }

        /// <summary>
        /// Creats the chat bubble and writes the text on it.
        /// </summary>
        public IEnumerator write()
        {
            this.instance = new GameObject("chat bubble");
            var source_sr = source.GetComponent<SpriteRenderer>();

            instance.transform.parent = source.transform;

            // Sprite Renderer

            var instance_sr = instance.AddComponent<SpriteRenderer>();
            instance_sr.sprite = generateSprite();
            instance_sr.spriteSortPoint = SpriteSortPoint.Pivot;
            instance_sr.sortingOrder = source_sr.sortingOrder + 2;

            // Transform

            instance.transform.position = source.transform.position;
            instance.transform.Translate(0, source_sr.sprite.rect.height / 16 + 0.5f, 0);

            // Create Text

            var textObject = new GameObject("text");
            textObject.transform.parent = instance.transform;
            var tmpComponent = textObject.AddComponent<TextMeshPro>();
            tmpComponent.text = text;
            tmpComponent.rectTransform.position = new Vector3(0, 0, 0);
            tmpComponent.alignment = TextAlignmentOptions.Center;
            tmpComponent.fontSize = 5;
            tmpComponent.color = Color.black;

            Vector2 size = calcSpriteSize();

            tmpComponent.rectTransform.sizeDelta = new Vector2((size.x - 2) / 16, (size.y - 4) / 16);
            tmpComponent.rectTransform.anchoredPosition = new Vector2(0, (size.y + 2) / 32);
            tmpComponent.sortingOrder = source_sr.sortingOrder + 3;

            yield return new WaitForSeconds(decay);

            onDecay();
            GameObject.Destroy(instance);
        }
    }
}