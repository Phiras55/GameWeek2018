using UnityEngine;
using System.Collections;

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
    [SerializeField] private float colMinHeight;

    private float       currentSpeed;
    private float       accumulatedTime;
    private Rigidbody   rb;

    private CapsuleCollider col;
    private float           colStartHeight;
    private int             strafeDir;

    private bool isGrounded;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityMultiplier;

    private bool isSliding = false;

    private int currentLane;
    private int startingLane;

    private Vector3 targetPos;

    // Use this for initialization
    private void Start ()
    {
        col                         = gameObject.GetComponent<CapsuleCollider>();
        colStartHeight              = col.height;
        currentLane                 = 0;
        rb = GetComponent<Rigidbody>();
        currentSpeed                = initialSpeed;
        accumulatedTime             = 0;
        Application.targetFrameRate = 60;

        currentLane = startingLane;
	}

    // Update is called once per frame
    private void Update()
    {
        if (Physics.OverlapSphere(transform.position - (Vector3.up), 0.1f, ~(1 << 8)).Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        MovePlayer();
        Strafe();
        Slide();
        Jump();
    }

    private void MovePlayer()
    {
        IncrementSpeed();

        transform.position  = transform.position + transform.forward.normalized * currentSpeed * Time.deltaTime;
    }

    private void IncrementSpeed()
    {
        accumulatedTime     += Time.deltaTime;
        if (accumulatedTime > incrementIntervalTime)
        {
            accumulatedTime = 0;
            currentSpeed    += speedIncrement;
            currentSpeed    = Mathf.Clamp(currentSpeed, initialSpeed, maxSpeed);
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

    private void Slide()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isSliding)
            isSliding = true;

        if (isSliding)
            StartCoroutine(Sliding());

        if(!isSliding)
        {
            col.height = Mathf.Lerp(col.height, colStartHeight, 5 * Time.deltaTime);

            if (Mathf.Approximately(col.height, colStartHeight))
                col.height = colStartHeight;
        }

    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        if (!isGrounded)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravityMultiplier, rb.velocity.z);
    }

    IEnumerator Sliding()
    {
        //isSliding = true;

        while(col.height > colMinHeight)
        {
            col.height -= 2 * Time.deltaTime;// Mathf.Lerp(col.height, colMinHeight, 2 * Time.deltaTime);
            
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        isSliding = false;
    }
}
