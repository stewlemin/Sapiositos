using UnityEngine;
using System.Collections.Generic;

public class Artifact : MonoBehaviour
{
    public static List<Artifact> all = new List<Artifact>(); 
    
    void OnEnable()
    {
        all.Add(this); 
    }

    void OnDisable()
    {
        all.Remove(this); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Artifact collected!");
            Destroy(gameObject);
        }
    }
}