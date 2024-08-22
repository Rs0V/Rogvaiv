using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SFINISH : MonoBehaviour
{
    public GameObject[] rings;

	private Vector2[] rotDir;
	private float[] rotSpeed;

	public SceneAsset next_level;

	private void Start()
	{
		rotDir = new Vector2[rings.Length];
		rotSpeed = new float[rings.Length];

		for (int i = 0; i < rings.Length; ++i)
		{
			float x = Random.value;
			float y = Random.value;

			rotDir[i] = new Vector2(x, y);
			rotSpeed[i] = Mathf.Clamp(Random.value + .4f, 0f, 1f) * 100f;
		}
	}

	private void Update()
	{
		for (int i = 0; i < rings.Length; ++i)
		{
			rings[i].transform.Rotate(new Vector3(rotDir[i].x, 0f, rotDir[i].y), rotSpeed[i] * Time.deltaTime);
		}
	}

	public void NextLevel(float delay)
	{
		StartCoroutine(GoToNextLevel(delay));
	}

	private IEnumerator GoToNextLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(next_level.name);
	}
}
