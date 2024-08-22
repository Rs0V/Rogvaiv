using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SCOLOR))]
public class COLOR_CHANGE_EDITOR : Editor
{
	int selected;
	string[] options; //System.Enum.GetNames(typeof(SCOLOR.ECOLOR));

	SCOLOR script;

	private void OnEnable()
	{
		script = target as SCOLOR;
		options = script.COLORS;
		selected = System.Array.IndexOf(script.MATS, script.GetComponent<Renderer>().sharedMaterial);
		selected = (selected < 0) ? System.Array.IndexOf(options, script.COLOR) : selected;
		script.COLOR = options[selected];
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		EditorGUI.BeginChangeCheck();

		selected = EditorGUILayout.Popup("Color", selected, options);

		if (EditorGUI.EndChangeCheck())
		{
			script.COLOR = options[selected];
			script.ChangeColor();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(script);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
		}
	}
}
