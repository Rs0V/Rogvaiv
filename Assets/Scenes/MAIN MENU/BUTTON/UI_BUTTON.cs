using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UI_BUTTON : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public GameObject label;
	
	private Material matInst = null;
	//private Color initGlowColor;
	private Color initOutlineColor;

	public bool QuitButton = false;
	public SceneAsset level = null;

	private enum PointerStatus
	{
		OVER,
		OUT,
		DOWN
	}
	private PointerStatus pointer = PointerStatus.OUT;

	// Start is called before the first frame update
	void Start()
	{
		matInst = label.GetComponent<TextMeshProUGUI>().fontMaterial;
		//initGlowColor = label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.GetColor("_GlowColor");
		initOutlineColor = label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.GetColor("_OutlineColor");

		if (QuitButton == false && level == null)
		{
			var data = SaveSystem.LoadData();
			if (data == null)
				transform.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		//label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_GlowColor", new Color(.4f, .4f, .4f, .5f));

		label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.EnableKeyword("OUTLINE_ON");
		label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_OutlineColor", initOutlineColor);

		pointer = PointerStatus.OVER;
	}

	public void OnPointerExit(PointerEventData pointerEventData)
	{
		//label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_GlowColor", initGlowColor);

		label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_OutlineColor", initOutlineColor);
		label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.DisableKeyword("OUTLINE_ON");

		pointer = PointerStatus.OUT;
	}

	public void OnPointerDown(PointerEventData pointerEventData)
	{
		//label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_GlowColor", new Color(.8f, .8f, .8f, .5f));

		label.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_OutlineColor", Color.white);

		pointer = PointerStatus.DOWN;
	}

	public void OnPointerUp(PointerEventData pointerEventData)
	{
		if (pointer == PointerStatus.DOWN)
		{
			OnPointerEnter(null);
			if (QuitButton == false)
				if (level != null)
					SceneManager.LoadScene(level.name);
				else
				{
					var data = SaveSystem.LoadData();
					if (data != null)
						SceneManager.LoadScene(data.level);
				}
			else
				Application.Quit();
		}
	}
}