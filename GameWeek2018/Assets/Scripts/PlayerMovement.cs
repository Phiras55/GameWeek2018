using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Attributes")]
    [SerializeField] private float initialSpeed             = 1;
    [SerializeField] private float speedIncrement           = 0.0001f;
    [SerializeField] private float incrementIntervalTime    = 0;
    [SerializeField] private float maxSpeed                 = 10;

    [Header("Strafe Attributes")]
    [SerializeField] private float strafeDistance;
    [SerializeField] private float strafeSpeed;

    private float       currentSpeed;
    private float       accumulatedTime;
    private Rigidbody   rigidbody;

    private int         strafeDir;

    private bool        right       = false;
    private bool        left        = false;
    private bool        isStrafing  = false;

    private Vector3 targetPos;

    // Use this for initialization
    private void Start ()
    {
        rigidbody                   = GetComponent<Rigidbody>();
        currentSpeed                = initialSpeed;
        accumulatedTime             = 0;
        Application.targetFrameRate = 60;
	}

    // Update is called once per frame
    private void Update()
    {
        MovePlayer();
        Strafe();
    }

    private void MovePlayer()
    {
        IncrementSpeed();
        transform.position = transform.position + transform.forward.normalized * currentSpeed * Time.deltaTime;
        Debug.Log("Speederu! " + currentSpeed);
    }

    private void IncrementSpeed()
    {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > incrementIntervalTime)
        {
            accumulatedTime = 0;
            currentSpeed += speedIncrement;
            currentSpeed = Mathf.Clamp(currentSpeed, initialSpeed, maxSpeed);
            Debug.Log("Speed Incremented! " + currentSpeed);
        }
    }



    private void Strafe()
    {

        if(Input.GetKeyDown(KeyCode.D))
        {
            if (left)
                left = false;
            else
                right = true;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            if (right)
                right = false;
            else
                left = true;
        }

        if(left)
        {
            targetPos = transform.position - (transform.right * strafeDistance);
        }
        else if(right)
        {
            targetPos = transform.position + (transform.right * strafeDistance);
        }
        else
        {
            targetPos = transform.position;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, strafeSpeed * Time.deltaTime);



    }
}
