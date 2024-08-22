using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPIKE : MonoBehaviour
{
    public enum facing_options
    {
        UP = 0,
        RIGHT = 90,
        DOWN = 180,
        LEFT = 270
    }

    [HideInInspector]
    public facing_options facing = facing_options.UP;

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
}
