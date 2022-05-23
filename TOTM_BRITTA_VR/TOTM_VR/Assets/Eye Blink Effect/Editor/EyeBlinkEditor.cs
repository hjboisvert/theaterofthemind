using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PostProcess
{
	[CustomEditor (typeof(BlinkEffect))]
	public class EyeBlinkEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			BlinkEffect effect = (BlinkEffect) target;
			if (effect.isActiveAndEnabled) {
				if (GUILayout.Button ("Test Animation")) {
					if (!Application.isPlaying) 
						effect.RunEditorPreview ();
					else 
						effect.Blink ();
				}
			} else {
				GUILayout.Space (5);
				GUILayout.Label ("Camera is inactive");
			}
		}
	}
}