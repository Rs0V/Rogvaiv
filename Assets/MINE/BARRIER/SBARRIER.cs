using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBARRIER : MonoBehaviour
{
	public GameObject mesh_h;
	public GameObject mesh_v;

	[HideInInspector]
	public GameObject active_mesh;

	[HideInInspector]
	public string TYPE;

	[HideInInspector]
	public string COLOR;

	public string[] COLORS = { "RED", "ORANGE", "YELLOW", "GREEN", "BLUE", "INDIGO", "PURPLE" };
	public Material[] MATS;

	private Coroutine lastPulseHitRoutine = null;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void ChangeOrientation()
	{
		if (TYPE == "HORIZONTAL")
		{
			mesh_h.SetActive(true);
			mesh_v.SetActive(false);
			active_mesh = mesh_h;
		}
		else
		{
			mesh_h.SetActive(false);
			mesh_v.SetActive(true);
			active_mesh = mesh_v;
		}
	}

	public void ChangeColor()
	{
		mesh_h.GetComponent<Renderer>().sharedMaterial = MATS[System.Array.IndexOf(COLORS, COLOR)];
		mesh_v.GetComponent<Renderer>().sharedMaterial = MATS[System.Array.IndexOf(COLORS, COLOR)];
	}

	public void PulseHit()
	{
		if (lastPulseHitRoutine != null)
			StopCoroutine(lastPulseHitRoutine);
		lastPulseHitRoutine = StartCoroutine(PulseColorANDAlpha(Color.white * 3f, 1f, 0f, 0f, .5f));
	}

	private Material matInst = null;
	private IEnumerator PulseColorANDAlpha(Color new_color, float new_alpha, float fade_in, float hold, float fade_out)
	{
		float fi_time = 0f, fo_time = 0f;

		Material orig_mat = MATS[System.Array.IndexOf(COLORS, COLOR)];
		Color barrier_albedo = orig_mat.color;
		float old_alpha = barrier_albedo.a;
		Color old_color = orig_mat.GetColor("_EmissionColor");

		if (matInst != null)
			DestroyImmediate(matInst);

		active_mesh.GetComponent<Renderer>().sharedMaterial = orig_mat;
		matInst = active_mesh.GetComponent<Renderer>().material;

		while (fi_time < fade_in)
		{
			active_mesh.GetComponent<Renderer>().sharedMaterial.color
				= new Color(barrier_albedo.r, barrier_albedo.g, barrier_albedo.b, Mathf.Lerp(old_alpha, new_alpha, fi_time / fade_in));

			active_mesh.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.Lerp(old_color, new_color, fi_time / fade_in));

			fi_time += Time.deltaTime;
			yield return null;
		}

		active_mesh.GetComponent<Renderer>().sharedMaterial.color = new Color(barrier_albedo.r, barrier_albedo.g, barrier_albedo.b, new_alpha);
		active_mesh.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", new_color);
		if (hold > 0f)
			yield return new WaitForSeconds(hold);

		while (fo_time < fade_out)
		{
			active_mesh.GetComponent<Renderer>().sharedMaterial.color
				= new Color(barrier_albedo.r, barrier_albedo.g, barrier_albedo.b, Mathf.Lerp(new_alpha, old_alpha, fo_time / fade_out));

			active_mesh.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.Lerp(new_color, old_color, fo_time / fade_out));

			fo_time += Time.deltaTime;
			yield return null;
		}

		DestroyImmediate(matInst);
		active_mesh.GetComponent<Renderer>().sharedMaterial = orig_mat;
	}
}
