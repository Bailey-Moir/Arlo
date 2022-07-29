using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Arlo.Editors
{
    /// <summary>
    /// The editor for the main camera.
    /// </summary>
    [CustomEditor(typeof(MainCamera)), ExecuteInEditMode]
    public class MainCameraEditor : Editor
    {
        /// <summary>
        /// The original script.
        /// </summary>
        private MainCamera script;
        /// <summary>
        /// The PixelPerfectCamera of the script's camera.
        /// </summary>
        private PixelPerfectCamera ppc;
        /// <summary>
        /// If the script should be drawing the bounding box.
        /// </summary>
        private bool _drawBox = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Toggle Rectangle"))
            {
                _drawBox = !_drawBox;
            }
        }

        public void OnEnable()
        {
            script = (MainCamera)target;
            ppc = script.GetComponent<PixelPerfectCamera>();
        }

        void OnSceneGUI()
        {
            if (_drawBox)
            {
                var left = script.cameraBoundingBox.position.x - script.cameraBoundingBox.width / 2;
                var right = script.cameraBoundingBox.position.x + script.cameraBoundingBox.width / 2;
                var bottom = script.cameraBoundingBox.position.y - script.cameraBoundingBox.height / 2;
                var top = script.cameraBoundingBox.position.y + script.cameraBoundingBox.height / 2;

                var margin = new Vector2(ppc.refResolutionX, ppc.refResolutionY) / (float)ppc.assetsPPU / 2;

                // Outside

                // Top
                Debug.DrawLine(new Vector2(right, top), new Vector2(left, top), Color.white, 0.1f);
                // Bottom
                Debug.DrawLine(new Vector2(right, bottom), new Vector2(left, bottom), Color.white, 0.1f);
                // left
                Debug.DrawLine(new Vector2(left, top), new Vector2(left, bottom), Color.white, 0.1f);
                // Right
                Debug.DrawLine(new Vector2(right, top), new Vector2(right, bottom), Color.white, 0.1f);
                
                // Inside

                // Top
                Debug.DrawLine(new Vector2(right - margin.x, top - margin.y), new Vector2(left + margin.x, top - margin.y), Color.red, 0.1f);
                // Bottom
                Debug.DrawLine(new Vector2(right - margin.x, bottom + margin.y), new Vector2(left + margin.x, bottom + margin.y), Color.red, 0.1f);
                // left
                Debug.DrawLine(new Vector2(left + margin.x, top - margin.y), new Vector2(left + margin.x, bottom + margin.y), Color.red, 0.1f);
                // Right
                Debug.DrawLine(new Vector2(right - margin.x, top - margin.y), new Vector2(right - margin.x, bottom + margin.y), Color.red, 0.1f);
            }
        }
    }
}
