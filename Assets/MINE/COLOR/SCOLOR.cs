using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SCOLOR : MonoBehaviour
{
	/*
	public enum ECOLOR
	{
		RED,
		ORANGE,
		YELLOW,
		GREEN,
		BLUE,
		INDIGO,
		PURPLE
	}

	[HideInInspector]
	public ECOLOR COLOR;
	*/

	[HideInInspector]
	public string COLOR;

	public string[] COLORS = { "RED", "ORANGE", "YELLOW", "GREEN", "BLUE", "INDIGO", "PURPLE" };
	public Material[] MATS;

	private Vector2 rotDir;
	private float rotSpeed;

	private void Start()
	{
		float x = Random.value;
		float y = Random.value;

		rotDir = new Vector2(x, y);
		rotSpeed = Mathf.Clamp(Random.value + .4f, 0f, 1f) * 100f;
	}

	private void Update()
	{
		transform.Rotate(new Vector3(rotDir.x, 0f, rotDir.y), rotSpeed * Time.deltaTime);
	}

	public void ChangeColor()
	{
		GetComponent<Renderer>().sharedMaterial = MATS[System.Array.IndexOf(COLORS, COLOR)];
	}
}

/*
	Color new_color = Color.white;
	switch (COLOR)
	{
		case ECOLOR.RED:
			new_color = Color.red;
			break;
		case ECOLOR.ORANGE:
			new_color = new Color(1f, .125f, .025f);
			break;
		case ECOLOR.YELLOW:
			new_color = Color.yellow;
			break;
		case ECOLOR.GREEN:
			new_color = Color.green;
			break;
		case ECOLOR.BLUE:
			new_color = new Color(0f, .15f, 1f);
			break;
		case ECOLOR.INDIGO:
			new_color = new Color(.1f, .05f, 1f);
			break;
		case ECOLOR.PURPLE:
			new_color = new Color(.15f, 0f, 1f);
			break;
	}
	GetComponent<Renderer>().material.enableInstancing = true;
	GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
	GetComponent<Renderer>().material.color = Color.black;
	GetComponent<Renderer>().material.SetColor("_EmissionColor", new_color * 2);

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////

	Material red = (Material)Resources.Load("Assets/MINE/COLOR/MRED.mat", typeof(Material));
	Material orange = (Material)Resources.Load("Assets/MINE/COLOR/MORANGE.mat", typeof(Material));
	Material yellow = (Material)Resources.Load("Assets/MINE/COLOR/MYELLOW.mat", typeof(Material));
	Material green = (Material)Resources.Load("Assets/MINE/COLOR/MGREEN.mat", typeof(Material));
	Material blue = (Material)Resources.Load("Assets/MINE/COLOR/MBLUE.mat", typeof(Material));
	Material indigo = (Material)Resources.Load("Assets/MINE/COLOR/MINDIGO.mat", typeof(Material));
	Material purple = (Material)Resources.Load("Assets/MINE/COLOR/MPURPLE.mat", typeof(Material));
*/
