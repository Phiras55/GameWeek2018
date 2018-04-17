using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.localPosition = Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        other.transform.localPosition = Vector3.zero;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.localPosition = Vector3.zero;
    }
}
