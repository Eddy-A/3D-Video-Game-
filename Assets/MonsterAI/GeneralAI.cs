using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GeneralAI : FSM
{
    public enum FSMState
    {
        None,
        Patrol,
        Stare,
        Chase,
        Manic,
        Attack,
        Dead,
    }
    [SerializeField]
    PlayerStats PlayerFunc;
    [SerializeField]
    ThirdPersonMovement PlayerMove;


    //Current state that the NPC is reaching
    public FSMState curState;

    // time
    private float elapsedTime;
    //Speed of the tank
    private float curSpeed;
    public float atkCooldownTimeMain;
    public float atkCooldownTime;
    public float atkPower;
    public float atkRange;
    public float sightRange;
    public float chaseRange;

    //Tank Rotation Speed
    private float curRotSpeed;

    //Whether the NPC is destroyed or not
    private bool bDead;
    public int health;
    public float maxHealth;
    //public float jumpHeight = 3f;


    private Rigidbody rbody;
    private Text healText;
    private Image healBar;
    private int curPoint;
    public Animator animator;
    public GameObject hitBox;
    public bool isBiting = false;
    public bool hasAtt = false;
    //public TextMeshProUGUI enemyCount;
    //public EnemyCount enemyLeft;
    //public GameObject explostion;
    //private int goldCount;


    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        curState = FSMState.Patrol;
        curSpeed = 3.0f;
        curRotSpeed = 20.0f;
        bDead = false;
        elapsedTime = 0.0f;
        healText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
        healBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
        //goldCount = Player.GetComponent<PlayerGold>().goldCnt;

        rbody = GetComponent<Rigidbody>();

        //Find next destination point
        curPoint = 0;
        FindNextPoint(curPoint);

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

    }
    public void ActivateHitBox() //assign Tag to the part of the monster that has the collider
    {
        hitBox.GetComponent<Collider>().enabled = true;
    }

    public void DeactivateHitBox()
    {
        hitBox.GetComponent<Collider>().enabled = false;
    }

    public int GetHealth()
    {
        return health;
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        healText.text = health.ToString();
        healBar.fillAmount = health / maxHealth;
        // update gold count
        //goldCount = Player.GetComponent<PlayerGold>().goldCnt;

        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Stare: UpdateStareState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Manic: UpdateManicState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //Update the time
        elapsedTime += Time.deltaTime;

        //Go to dead state is no health left
        if (health <= 0)
        {
            health = 0;
            curState = FSMState.Dead;
        }

    }

    /// <summary>
    /// Patrol state
    /// </summary>
    protected void UpdatePatrolState()
    {
        //Set the target position as the patrol points
        destPos = pointList[curPoint];

        // Check if manic state reached (more than half the gold obtained)
        if (health <= (maxHealth / 2))
        {
            print("Switch to Manic Position");
            animator.SetBool("isChasing", true);
            curState = FSMState.Manic;
        }

        //Find another random patrol point if the current point is reached
        else if (Vector3.Distance(transform.position, pointList[curPoint]) <= 1.0f)
        {
            print("Reached to the destination point\ncalculating the next point");
            curPoint = (curPoint + 1) % 2;
            FindNextPoint(curPoint);
        }

        //Check the distance with player
        //When the distance is near, transition to stare state
        else if (Vector3.Distance(transform.position, playerTransform.position) <= sightRange)
        {
            print("Switch to Stare Position");
            animator.SetBool("sightedPlayer", true);
            curState = FSMState.Stare;
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        transform.Translate(Time.deltaTime * curSpeed * Vector3.forward);
    }

    /// <summary>
    /// Stare state
    /// </summary>
    protected void UpdateStareState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with player
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        // Check if manic state reached (more than half the gold obtained)
        if (health <= (maxHealth / 2))
        {
            print("Switch to Manic Position");
            animator.SetBool("isChasing", true);
            curState = FSMState.Manic;
        }
        //Go to chase if close enough
        else if (dist <= chaseRange)
        {
            animator.SetBool("isChasing", true);
            curState = FSMState.Chase;
        }
        //Go back to patrol is it become too far
        else if (dist >= sightRange)
        {
            animator.SetBool("sightedPlayer", false);
            curState = FSMState.Patrol;
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
    }

    /// <summary>
    /// Chase state
    /// </summary>
    protected void UpdateChaseState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        // Check if manic state reached (more than half the gold obtained)
        if (health <= (maxHealth / 2))
        {
            print("Switch to Manic Position");
            animator.SetBool("isChasing", true);
            curState = FSMState.Manic;
        }

        else if (dist <= atkRange)
        {
            Debug.Log("Now Atking");
            transform.position = this.transform.position;
            if (atkCooldownTime > 0)
            {
                atkCooldownTime -= Time.deltaTime;
            }

            else
            {
                transform.position = this.transform.position;
                animator.SetTrigger("Attacking");
                atkCooldownTime = atkCooldownTimeMain;
            }

            curState = FSMState.Attack;
        }
        //Transition to Chase if within distance
        else if (dist <= chaseRange && dist > atkRange)
        {
            Debug.Log("Now Chasing");
            //Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            transform.Translate(Time.deltaTime * curSpeed * Vector3.forward);

            curState = FSMState.Chase;
        }
        //Transition to stare is the tank become too far
        else if (dist > chaseRange && dist < sightRange)
        {
            animator.SetBool("isChasing", false);
            curState = FSMState.Stare;
        }
        //Transition to patrol if the tank become too far
        else if (dist >= sightRange)
        {
            animator.SetBool("sightedPlayer", false);
            curState = FSMState.Patrol;
        }
    }

    protected void UpdateManicState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        if (dist <= atkRange)
        {
            animator.SetBool("isChasing", false);
            transform.position = this.transform.position;
            if (atkCooldownTime > 0)
            {
                atkCooldownTime -= Time.deltaTime;
            }

            else
            {
                animator.SetTrigger("Attacking");
                atkCooldownTime = atkCooldownTimeMain;
            }
            curState = FSMState.Attack;

        }
        else
        {
            //Go Forward
            animator.SetBool("isChasing", true);
            transform.Translate(Time.deltaTime * curSpeed * Vector3.forward);
            curState = FSMState.Manic;
        }
    }

    protected void UpdateAttackState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        if (dist > atkRange)
        {
            print("Switch to Manic Position");
            animator.SetBool("isChasing", true);
            curState = FSMState.Chase;
        }

        else
        {
            transform.position = this.transform.position;
            if (atkCooldownTime > 0)
            {
                atkCooldownTime -= Time.deltaTime;
            }

            else
            {
                animator.SetTrigger("Attacking");
                atkCooldownTime = atkCooldownTimeMain;
            }
            curState = FSMState.Attack;
        }

    }

    /// <summary>
    /// Dead state
    /// </summary>
    protected void UpdateDeadState()
    {
        //Show the dead animation with some physics effects
        if (!bDead)
        {
            bDead = true;
            animator.SetBool("isDead", true);
            Invoke(nameof(Explode), 3);
        }
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.CompareTag("Bullet") && PlayerMove.isAttacking == true)
        {
            animator.SetTrigger("wasHit");
            health -= PlayerFunc.ATT;
        }

        if (health <= 0)
        {
            PlayerFunc.GainEXP(30);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Step"))
        {
            //Debug.Log("Entered zone");
            rbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }*/

    /// <summary>
    /// Find the next semi-random patrol point
    /// </summary>
    /// 

    protected void FindNextPoint(int nextPoint)
    {
        print("Finding next point");

        destPos = pointList[nextPoint];
    }

    protected void Explode()
    {
        Destroy(gameObject, 1.5f);
    }

    public void Biting()
    {
        isBiting = !isBiting;
        if (hasAtt == true)
            hasAtt = false;
        animator.SetBool("Biting", isBiting);
    }



}
