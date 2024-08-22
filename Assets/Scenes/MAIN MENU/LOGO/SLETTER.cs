using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLETTER : MonoBehaviour
{
	public Color startColor = Color.red;
	public float hueSpeed = 1f;

	private Material matInst = null;

	private Vector3 initPos;
	public float moveSpeed = 1f;

	public float delay = 0f;
	private float timer = 0f;
	private float offset = -1f;

	// Start is called before the first frame update
	void Start()
	{
		matInst = GetComponent<Renderer>().material;
		GetComponent<Renderer>().sharedMaterial.color = startColor;

		initPos = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		SPLAYER.HSV hsv = Color2HSV(GetComponent<Renderer>().sharedMaterial.color);
		GetComponent<Renderer>().sharedMaterial.color = hsv.Hue_Abs(hsv.h - hueSpeed * Time.deltaTime).ToColor();

		if(offset < 0f)
		{
			if (timer > delay)
			{
				offset = Time.time + delay;
				transform.position = Vector3.Lerp(initPos, initPos + new Vector3(0f, 0f, .2f), (Mathf.Sin((Time.time + delay - offset) * moveSpeed) + 1f) * .5f);
			}
			else
				timer += Time.deltaTime;
		}
		else
			transform.position = Vector3.Lerp(initPos, initPos + new Vector3(0f, 0f, .2f), (Mathf.Sin((Time.time + delay - offset) * moveSpeed) + 1f) * .5f);
	}

	public SPLAYER.HSV Color2HSV(in Color color)
	{
		SPLAYER.HSV hsv = new SPLAYER.HSV();
		Color.RGBToHSV(color, out hsv.h, out hsv.s, out hsv.v);
		return hsv;
	}
}
