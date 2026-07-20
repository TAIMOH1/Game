using UnityEngine;

public class CollectObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;

        Debug.Log("Entered by: " + other.name);
        Debug.Log("Root object: " + root.name);

        if (root.CompareTag("Player"))
        {
            Debug.Log("Collected!");
            Destroy(gameObject);
        }
    }
}