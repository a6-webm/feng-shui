using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Edge edge = GetComponent<Edge>();
        if (edge == null) { return; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
