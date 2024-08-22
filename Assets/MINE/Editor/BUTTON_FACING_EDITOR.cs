using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SBUTTON))]
public class BUTTON_FACING_EDITOR : Editor
{
	int selected;
	string[] options = { "UP", "RIGHT", "DOWN", "LEFT" };

	SBUTTON script;

	private string Facing2String(SBUTTON.facing_options facing)
	{
		switch (facing)
		{
			case SBUTTON.facing_options.UP:
				return "UP";
			case SBUTTON.facing_options.RIGHT:
				return "RIGHT";
			case SBUTTON.facing_options.DOWN:
				return "DOWN";
			case SBUTTON.facing_options.LEFT:
				return "LEFT";
			default:
				return "NONE";
		}
	}

	private SBUTTON.facing_options String2Facing(string facing_name)
	{
		switch (facing_name)
		{
			case "UP":
				return SBUTTON.facing_options.UP;
			case "RIGHT":
				return SBUTTON.facing_options.RIGHT;
			case "DOWN":
				return SBUTTON.facing_options.DOWN;
			case "LEFT":
				return SBUTTON.facing_options.LEFT;
			default:
				return SBUTTON.facing_options.UP;
		}
	}

	private void OnEnable()
	{
		script = target as SBUTTON;

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
