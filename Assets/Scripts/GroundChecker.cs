using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player.gameObject);
        return;
        player.GroundState(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player.gameObject);
        return;
        player.GroundState(false);
    }
}
