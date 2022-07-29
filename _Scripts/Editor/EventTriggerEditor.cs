using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Arlo.Editors
{
    /// <summary>
    /// The editor for the event trigger.
    /// </summary>
    [CustomEditor(typeof(EventTrigger))]
    public class EventTriggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EventTrigger script = (EventTrigger) target;
            if (GUILayout.Button("Trigger"))
            {
                script.onEnter.Invoke();
            }
        }
    }
}
