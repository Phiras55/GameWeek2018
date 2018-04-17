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

    private int currentLane;
    private int startingLane;

    private Vector3 targetPos;

    // Use this for initialization
    private void Start ()
    {
        currentLane = 0;
        rigidbody                   = GetComponent<Rigidbody>();
        currentSpeed                = initialSpeed;
        accumulatedTime             = 0;
        Application.targetFrameRate = 60;

        currentLane = startingLane;
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

        //targetPos += transform.position + (transform.forward.normalized * (currentLane * Time.deltaTime));
        transform.position = transform.position + transform.forward.normalized * currentSpeed * Time.deltaTime;

        //Debug.Log("Speederu! " + currentSpeed);

        //transform.position = targetPos;
    }

    private void IncrementSpeed()
    {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > incrementIntervalTime)
        {
            accumulatedTime = 0;
            currentSpeed += speedIncrement;
            currentSpeed = Mathf.Clamp(currentSpeed, initialSpeed, maxSpeed);
            //Debug.Log("Speed Incremented! " + currentSpeed);
        }
    }



    private void Strafe()
    {
        targetPos = Vector3.zero;
        if(Input.GetKeyDown(KeyCode.D))
        {
            ChangeLane(1);
            transform.position += targetPos;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            ChangeLane(-1);
            transform.position += targetPos;
        }
       
    }

    private void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;

        if (targetLane < 0 || targetLane > 2)
            return;

        currentLane = targetLane;

        Vector3 deltapos = new Vector3((currentLane - 1), 0, 0);

        if (deltapos.x == 0 && direction == 1)
            deltapos.x = 1;
        else if (deltapos.x == 0 && direction == -1)
            deltapos.x = -1;

        targetPos = transform.rotation * deltapos * strafeDistance;

        Debug.Log(targetPos);
    }
}
