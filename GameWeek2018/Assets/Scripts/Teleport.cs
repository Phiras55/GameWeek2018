using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 origin = new Vector3(0, other.transform.parent.position.y, 0);
            other.transform.parent.position = origin;
            Debug.Log("Player");
        }

        //other.transform.Rotate(Vector3.up, 90);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 origin = new Vector3(0, other.transform.parent.position.y, 0);
            other.transform.parent.position = origin;
            Debug.Log("Player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 origin = new Vector3(0, other.transform.parent.position.y, 0);
            other.transform.parent.position = origin;
            Debug.Log("Player");
        }
    }
}
