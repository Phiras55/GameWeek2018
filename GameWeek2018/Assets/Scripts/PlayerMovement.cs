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
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityMultiplier;

    private float           currentSpeed;
    private float           accumulatedTime;
    private Rigidbody       rb;
    private RaycastHit      hit;

    private CapsuleCollider col;
    private float           colStartHeight;
    private int             strafeDir;

    private Animator        anim;

    private Obstacle        currentObstacle = null;


    private bool            isGrounded, isJumping;


    private bool            isSliding                       = false;
    private bool            isStrafing                      = false;
    private bool            isClimbing                      = false;

    private int             currentLane;
    private int             startingLane                    = 1;
    private Vector3         targetPos;
    public Transform[]      lanes;
    public float            groundOffset;

    private float           minCheckTime;


    [HideInInspector]
    public bool             canMove = true;

    // Use this for initialization
    private void Start ()
    {
        anim                                = gameObject.GetComponent<Animator>();
        col                                 = gameObject.GetComponent<CapsuleCollider>();
        colStartHeight                      = col.height;
        currentLane                         = 0;
        rb = GetComponent<Rigidbody>();
        currentSpeed                        = initialSpeed;
        accumulatedTime                     = 0;
        Application.targetFrameRate         = 60;

        currentLane = startingLane;

        lanes[0].localPosition = -Vector3.right * strafeDistance;
        lanes[1].localPosition = Vector3.zero;
        lanes[2].localPosition = Vector3.right * strafeDistance;
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(isSliding);

        LookObstacle();
    }

    private void LookObstacle()
    {
        if(Physics.Raycast(transform.position + (transform.forward * 0.5f) + Vector3.up, transform.forward, out hit, currentSpeed / 2,~(1<<8), QueryTriggerInteraction.Ignore ))
        {
            if(hit.collider.gameObject.CompareTag("Obstacle"))
            {
                currentObstacle = hit.collider.gameObject.GetComponent<Obstacle>();
            }
        }
        else
        {
            currentObstacle = null;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Strafe();
        Slide();
        Jump();
        Climb();
        CheckGrounded();
        MovePlayer();
        Strafe();
        Slide();
        Jump();
        Climb();
    }

    private void MovePlayer()
    {
        if (!canMove)
            return;

        IncrementSpeed();

        transform.parent.position  += transform.parent.forward.normalized * (currentSpeed * Time.deltaTime);
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
        //targetPos = Vector3.zero;

        int lane = currentLane;

        if(Input.GetKeyDown(KeyCode.D) && !isStrafing)
        {
            lane = ChangeLane(1);
            //transform.position += targetPos;
        }
        else if(Input.GetKeyDown(KeyCode.A) && !isStrafing)
        {
            lane = ChangeLane(-1);
            //transform.position += targetPos;
        }

        Vector3 vec = new Vector3(lanes[lane].localPosition.x, transform.localPosition.y, 0);

        transform.localPosition =  Vector3.Lerp(transform.localPosition, vec, strafeSpeed * Time.deltaTime);
       
    }

    private int ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;

        if (targetLane < 0 || targetLane > 2)
            return currentLane;

        currentLane = targetLane;

        //Vector3 deltapos = new Vector3((currentLane - 1), 0, 0);

        /*if (deltapos.x == 0 && direction == 1)
            deltapos.x = 1;
        else if (deltapos.x == 0 && direction == -1)
            deltapos.x = -1;*/

        //targetPos = transform.rotation * deltapos * strafeDistance;

        //targetPos = deltapos;

        return currentLane;

        //Debug.Log(targetPos);
    }

    private void Slide()
    {

        if (Input.GetKeyDown(KeyCode.S) && !isSliding && !isJumping)
        {
            isSliding = true;
        }

        if (isSliding)
            StartCoroutine(Sliding());
        else
        {
            col.height = Mathf.Lerp(col.height, colStartHeight, 2 * Time.deltaTime);

            Vector3 newCenter = new Vector3(col.center.x, 0.5f, col.center.z);

            col.center = Vector3.Lerp(col.center, newCenter, 1 * Time.deltaTime);

            if (Mathf.Approximately(col.height, colStartHeight))
                col.height = colStartHeight;
        }

        anim.SetBool("Sliding", isSliding);
        //newval = (((oldval - oldmin) * (newmax - newmin)) / (oldmax - oldmin)) + newmin
        float animSpeed = (((currentSpeed - initialSpeed) * (2 - 1)) / (maxSpeed - initialSpeed)) + 1;

        if (!canMove)
            animSpeed = 0;

        anim.SetFloat("CurrentSpeed", animSpeed);

    }

    private void CheckGrounded()
    {
        if (Time.time < minCheckTime)
            return;

        if (Physics.OverlapSphere(transform.position - ((Vector3.up * groundOffset) * transform.localScale.y), 0.1f, ~(1 << 8)).Length > 0)
        {
            isGrounded = true;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }

        if (!isGrounded)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravityMultiplier, rb.velocity.z);
        //else
            //rb.velocity = new Vector3(rb.velocity.x, -0.5f, rb.velocity.z);
    }

    private void Jump()
    {
        //Debug.Log("Is jumping " + isJumping);
        if(Input.GetKeyDown(KeyCode.Space) && !isSliding && !isJumping)
        {
            if (currentObstacle)
            {
                if (((currentObstacle.obsType & Obstacle.E_ObsType.Climb) != 0))
                {
                    isClimbing = true;
                }
                else
                {
                    ApplyJump();
                }
            }
            else
            {
                ApplyJump();
            }
        }
        anim.SetBool("Jump", isJumping);
    }

    private void ApplyJump()
    {
        minCheckTime = Time.time + 0.5f;
        isJumping = true;
        isGrounded = false;
        //Debug.Log("Jumping");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    private void Climb()
    {
        anim.SetBool("Climb", isClimbing);
        if (!isClimbing)
            return;
        //if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Climb"))
        //{

        //}

        if (currentObstacle)
        {
            RaycastHit topHit;
            if(Physics.SphereCast((hit.point + (Vector3.up * 5f)), 0.5f, -Vector3.up, out topHit, 6f ))
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                col.enabled = false;

                transform.position = Vector3.Lerp(transform.position, topHit.point + (topHit.normal * 10f), 2f * Time.deltaTime);
            }
        }

    }

    public void SetClimbFalse()
    {
        Debug.Log("Deactivate Player Climb");
        isClimbing = false;

        StartCoroutine(Activating());

    }

    IEnumerator Activating()
    {
        yield return new WaitForSeconds(0.2f);

        if (!rb.useGravity)
            rb.useGravity = true;
        if (rb.isKinematic)
            rb.isKinematic = false;
        if (!col.enabled)
            col.enabled = true;

        currentObstacle = null;

        rb.AddForce(Vector3.up * 3f, ForceMode.VelocityChange);
    }

    IEnumerator Sliding()
    {
        //isSliding = true;


        while(col.height > colMinHeight)
        {
            col.height -= 2 * Time.deltaTime;// Mathf.Lerp(col.height, colMinHeight, 2 * Time.deltaTime);
            Vector3 newCenter = new Vector3(col.center.x, 0.05f, col.center.z);

            col.center = Vector3.Lerp(col.center, newCenter, 2 * Time.deltaTime);
            
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        while(Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.up,1f, ~(1<<8), QueryTriggerInteraction.Ignore))
        {
            isSliding = true;
            yield return null;
        }

        isSliding = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - ((Vector3.up * groundOffset) * transform.localScale.y), 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + (transform.forward * 0.5f) + Vector3.up, (transform.position + Vector3.up) + (transform.forward * currentSpeed / 2));
    }
}
