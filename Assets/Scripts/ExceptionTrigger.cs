using System;
using UnityEngine;

public class ExceptionTrigger : MonoBehaviour
{
    [SerializeField] private string exceptionMessage = "Intentional Lab Exception";
    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;

        if (hasTriggered)
            return;

        if (!root.CompareTag("Player"))
            return;

        hasTriggered = true;

        throw new Exception(exceptionMessage);
    }
}