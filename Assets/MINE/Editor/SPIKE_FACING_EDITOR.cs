using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SSPIKE))]
public class SPIKE_FACING_EDITOR : Editor
{
	int selected;
	string[] options = { "UP", "RIGHT", "DOWN", "LEFT" };

	SSPIKE script;

	private string Facing2String(SSPIKE.facing_options facing)
	{
		switch (facing)
		{
			case SSPIKE.facing_options.UP:
				return "UP";
			case SSPIKE.facing_options.RIGHT:
				return "RIGHT";
			case SSPIKE.facing_options.DOWN:
				return "DOWN";
			case SSPIKE.facing_options.LEFT:
				return "LEFT";
			default:
				return "NONE";
		}
	}

	private SSPIKE.facing_options String2Facing(string facing_name)
	{
		switch (facing_name)
		{
			case "UP":
				return SSPIKE.facing_options.UP;
			case "RIGHT":
				return SSPIKE.facing_options.RIGHT;
			case "DOWN":
				return SSPIKE.facing_options.DOWN;
			case "LEFT":
				return SSPIKE.facing_options.LEFT;
			default:
				return SSPIKE.facing_options.UP;
		}
	}

	private void OnEnable()
	{
		script = target as SSPIKE;

		selected = System.Array.IndexOf(options, Facing2String(script.facing));
		selected = (selected < 0) ? 0 : selected;
		script.facing = String2Facing(options[selected]);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		selected = EditorGUILayout.Popup("Facing", selected, options);

		if (EditorGUI.EndChangeCheck())
		{
			script.facing = String2Facing(options[selected]);
			script.ChangeFacing();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(script);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
		}
	}
}
