using System.Collections;
using UnityEngine;

namespace Arlo
{
    /// <summary>
    /// The class for the second scene (the granny scene)
    /// </summary>
    public class GrannyScene : MonoBehaviour
    {
        private int _numberCollected = 0;
        /// <summary>
        /// The number of items collected thus far.
        /// </summary>
        public int NumberCollected
        {
            get => _numberCollected;
            set
            {
                _numberCollected = value;
                // Set the bowl to the appropriate sprite.
                bowlSr.sprite = bowlSprites[Mathf.FloorToInt(value / 2f)];
                // If the value is 5, make the candle interactable.
                if (value == 5)
                {
                    candle.interactable = true;
                }
                // If all are collected, then finish the granny scene.
                if (value == 2*bowlSprites.Length - 1) StartCoroutine(FinishCoroutine());
            }
        }

        // DEPENDENCIES

        /// <summary>
        /// The GameObject for the grandmother.
        /// </summary>
        public GameObject granny;
        /// <summary>
        /// The GameObject for the medecine recipe.
        /// </summary>
        public GameObject medecineRecipe;
        /// <summary>
        /// The GameObject for the UI element that toggles the medecine recipe.
        /// </summary>
        public GameObject medecineRecipeToggle;
        /// <summary>
        /// The GameObject for the player in the scene;
        /// </summary>
        public GameObject player;
        /// <summary>
        /// The sorted sprites for the bowl.
        /// </summary>
        public Sprite[] bowlSprites;
        /// <summary>
        /// The sprite for the granny when she is dead.
        /// </summary>
        public Sprite deadGranny;
        /// <summary>
        /// The bowl's sprite renderer.
        /// </summary>
        public SpriteRenderer bowlSr;
        /// <summary>
        /// The camera of the scene.
        /// </summary>
        public MainCamera mainCamera;
        /// <summary>
        /// The candle interactable.
        /// </summary>
        public Interactable candle;
        /// <summary>
        /// The collider of the wall that disappears after the <see cref="candle"/> is interacted with.
        /// </summary>
        public BoxCollider2D backWallCollider;
        /// <summary>
        /// The exit trigger from this scene to the next.
        /// </summary>
        public EventTrigger exitTrigger;
        /// <summary>
        /// The incarnate instance in the scene.
        /// </summary>
        public Entities.Incarnate incarnate;

        /// <summary>
        /// The Coroutine that occurs is called by the <see cref="StartQuest"/> function to start the interaction with the grandmother.
        /// </summary>
        private IEnumerator QuestCoroutine()
        {
            yield return new ChatBubble("Oh, hello there child", granny, 2).write();
            yield return new ChatBubble("I ran out of healing potion,\nand I need some", granny, 3).write();
            yield return new ChatBubble("Can you make some for me?", granny, 2).write();
            medecineRecipeToggle.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            yield return new ChatBubble("This is the recipe", granny, 2).write();
            yield return new ChatBubble("Click E on items to\ncollect them", granny, 2f).write();
            yield return new ChatBubble("Oh, and if you see my three\ngrand kids, please send\nthem back inside", granny, 3f).write();
            Item.items.ForEach((item) => item.interactable = true);
        }

        /// <summary>
        /// The Coroutine that is called when the player collects all the items.
        /// </summary>
        private IEnumerator FinishCoroutine()
        {
            yield return new WaitUntil(() => Mathf.Abs((player.transform.position - granny.transform.position).magnitude) < 3);

            yield return new ChatBubble("Thank you!", granny, 2).write();

            // Lerp the bowl to the granny
            var time = Time.timeSinceLevelLoad;
            Vector2 origin = bowlSr.transform.position;
            Vector2 point = new Vector2(1, bowlSr.transform.position.y);
            for (float t = 0; t <= 1; t = (Time.timeSinceLevelLoad - time) / Mathf.Abs((point - origin).magnitude))
            {
                bowlSr.transform.position = Vector2.Lerp(origin, point, t);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(0.5f);

            bowlSr.sprite = bowlSprites[0];

            yield return new WaitForSeconds(1f);

            granny.GetComponent<SpriteRenderer>().sprite = deadGranny;

            yield return new WaitForSeconds(0.5f);

            incarnate.gameObject.SetActive(true);
            yield return new ChatBubble("you just killed that old lady!", incarnate.gameObject, 2).write();
            yield return new ChatBubble("and it wasn't even\nself defense this time", incarnate.gameObject, 2.5f).write();
            yield return new ChatBubble("good job", incarnate.gameObject, 1.5f).write();

            // Allows the player to leave
            exitTrigger.triggerable = true;

            incarnate.linearlyGo(new Vector3(incarnate.transform.position.x, incarnate.transform.position.y - 8, incarnate.transform.position.z));
        }

        /// <summary>
        /// Called by the trigger around the grand mother to initate the interaction.
        /// </summary>
        public void StartQuest()    
        {
            candle.interact = () => StartCoroutine(candleInteract());
            StartCoroutine(QuestCoroutine());
        }

        /// <summary>
        /// What happens when the candle is interacted with.
        /// </summary>
        IEnumerator candleInteract()
        {
            candle.interactable = false;

            // Screen Shake
            for (int i = 0; i < 200; i++)
            {
                mainCamera.offset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                yield return new WaitForSeconds(0.01f);
            }
            mainCamera.offset = Vector2.zero;
            yield return new WaitForSeconds(1.5f);

            // Move wall
            backWallCollider.GetComponent<BoxCollider2D>().enabled = false;
            var time = Time.timeSinceLevelLoad;
            Vector2 point = new Vector2(0, 3);
            for (float t = 0; t <= 1; t = (Time.timeSinceLevelLoad - time) / Mathf.Abs(point.magnitude)) {
                backWallCollider.transform.localPosition = Vector2.Lerp(Vector2.zero, point, t);
                yield return new WaitForSeconds(0.01f);
            }

            backWallCollider.gameObject.SetActive(false);
            // Allow the camrea to go into the lab.
            mainCamera.cameraBoundingBox = new Rect(0, 2.625f, 15.9375f, 11.375f);
        }

        /// <summary>
        /// The function used the recipe toggle to toggle displaying the recipe.
        /// </summary>
        public void ToggleRecipe()
        {
            medecineRecipe.SetActive(!medecineRecipe.activeSelf);
        }
    }
}
