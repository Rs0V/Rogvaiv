using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCUBE : MonoBehaviour
{
    private Vector3 rot;
    private float speed;

    public Color[] colors;
    private float colorChangeDelay;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rot = new Vector3(Random.value, Random.value, Random.value);
        speed = Random.value * 20f + 20f;

        colorChangeDelay = Random.value * 3f + 2f;
        GetComponent<Renderer>().sharedMaterial.color = colors[Mathf.FloorToInt(Random.value * colors.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rot * speed * Time.deltaTime);

        if (timer > colorChangeDelay)
        {
            colorChangeDelay = Random.value * 3f + 2f;
            timer = 0f;
            GetComponent<Renderer>().sharedMaterial.color = colors[Mathf.FloorToInt(Random.value * colors.Length)];
        }

        timer += Time.deltaTime;
    }
}
