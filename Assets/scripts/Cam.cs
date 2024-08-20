using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public float smooth;
    public Player player;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position,smooth);
    }
}
