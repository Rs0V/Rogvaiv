using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SBUTTON : MonoBehaviour
{
	public GameObject mesh;

	[HideInInspector]
	public bool activated = false;

	public enum facing_options
	{
		UP = -180,
		RIGHT = -90,
		DOWN = 0,
		LEFT = -270
	}

	[HideInInspector]
	public facing_options facing = facing_options.UP;

	[System.Serializable]
	public struct Movable
	{
		public GameObject obj;
		public Move[] moves;
		public bool destroy;
	}

	[System.Serializable]
	public struct Move
	{
		public Vector3 delta;
		public float time;
	}

	public Movable[] Actors;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ChangeFacing()
	{
		transform.rotation = Quaternion.Euler(0f, (float)facing, 0f);
	}

	private Material matInst = null;
	public void Activate(in Vector3 direction)
	{
		activated = true;

		mesh.transform.position = mesh.transform.position + direction * .1f;

		matInst = mesh.GetComponent<Renderer>().material;
		mesh.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.green * 3f);

		foreach (var actor in Actors)
		{
			StartCoroutine(MoveObjects(actor));
		}
	}

	private IEnumerator MoveObjects(Movable actor)
	{
		//Vector3[] orig_pos = (from actor in Actors select actor.obj.transform.position).ToArray();
		Vector3 orig_pos = actor.obj.transform.position;
		if (actor.destroy == true)
			actor.obj.layer = LayerMask.NameToLayer("IGNORE");

		foreach (var move in actor.moves)
		{
			float timer = 0f;
			while(timer < move.time)
			{
				timer += Time.deltaTime;
				actor.obj.transform.position = Vector3.Lerp(orig_pos, orig_pos + move.delta, timer / move.time);
				yield return null;
			}
			actor.obj.transform.position = orig_pos + move.delta;
		}
		if (actor.destroy == true)
			Destroy(actor.obj);
	}

	private void OnDestroy()
	{
		if (matInst != null)
			DestroyImmediate(matInst);
	}
}
