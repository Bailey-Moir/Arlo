using UnityEngine;
using UnityEngine.U2D;

namespace Arlo
{
    /// <summary>
    /// The class for the main camera.
    /// </summary>
    public class MainCamera : MonoBehaviour
    {
        /// <summary>
        /// The pixel perfect camera component.
        /// </summary>
        private PixelPerfectCamera ppc;
        /// <summary>
        /// The bounding box of the camera.
        /// </summary>
        public Rect cameraBoundingBox;
        /// <summary>
        /// The offset of the camera from the player's position. By default set to (0,0).
        /// </summary>
        public Vector2 offset = new Vector2(0, 0);

        void Awake() => this.ppc = GetComponent<PixelPerfectCamera>();

        void FixedUpdate()
        {
            var playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            // The width of the camera view.
            var cameraWidth = new Vector2(ppc.refResolutionX, ppc.refResolutionY) / (float) ppc.assetsPPU;

            // Calcualte the position the camera wants to be in.
            Vector3 cameraTarget = new Vector3(
                playerPos.x + offset.x<= cameraBoundingBox.position.x - (cameraBoundingBox.width - cameraWidth.x)/2
                    ? cameraBoundingBox.position.x - (cameraBoundingBox.width - cameraWidth.x)/2
                : playerPos.x + offset.x >= cameraBoundingBox.position.x + (cameraBoundingBox.width - cameraWidth.x)/2
                    ? cameraBoundingBox.position.x + (cameraBoundingBox.width - cameraWidth.x)/2
                : playerPos.x + offset.x,
                playerPos.y + 0.5f + offset.y <= cameraBoundingBox.position.y - (cameraBoundingBox.height - cameraWidth.y)/2
                    ? cameraBoundingBox.position.y - (cameraBoundingBox.height - cameraWidth.y)/2
                : playerPos.y + 0.5f + offset.y >= cameraBoundingBox.position.y + (cameraBoundingBox.height - cameraWidth.y)/2
                    ? cameraBoundingBox.position.y + (cameraBoundingBox.height - cameraWidth.y)/2
                : playerPos.y + 0.5f + offset.y,
                -10
            );

            transform.position = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime * 5);
        }
    }
}
