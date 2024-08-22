using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SBARRIER))]
public class BARRIER_ORIENTATION_EDITOR : Editor
{
	int mesh_selected;
	string[] mesh_options = { "HORIZONTAL", "VERTICAL" };

	int color_selected;
	string[] color_options;

	SBARRIER script;

	private void OnEnable()
	{
		script = target as SBARRIER;

		mesh_selected = System.Array.IndexOf(mesh_options, script.TYPE);
		mesh_selected = (mesh_selected < 0) ? 0 : mesh_selected;
		script.TYPE = mesh_options[mesh_selected];

		color_options = script.COLORS;
		color_selected = System.Array.IndexOf(script.MATS, script.active_mesh.GetComponent<Renderer>().sharedMaterial);
		color_selected = (color_selected < 0) ? System.Array.IndexOf(color_options, script.COLOR) : color_selected;
		script.COLOR = color_options[color_selected];
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		mesh_selected = EditorGUILayout.Popup("Orientation", mesh_selected, mesh_options);
		color_selected = EditorGUILayout.Popup("Color", color_selected, color_options);

		if (EditorGUI.EndChangeCheck())
		{
			script.TYPE = mesh_options[mesh_selected];
			script.ChangeOrientation();

			script.COLOR = color_options[color_selected];
			script.ChangeColor();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(script);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
		}
	}
}
