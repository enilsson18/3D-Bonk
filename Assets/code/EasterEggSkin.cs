using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggSkin : MonoBehaviour
{
    public Material material;

    private void OnTriggerEnter(Collider collision)
    {
        collision.transform.gameObject.GetComponent<MeshRenderer>().material = material;
        print("changed skin");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
