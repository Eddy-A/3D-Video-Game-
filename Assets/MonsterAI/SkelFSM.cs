using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkelFSM : FSM 
{
    public enum FSMState
    {
        None,
        Spawn,
        Stare,
        Manic,
        Attack,
        Dead,
    }

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
    //public TextMeshProUGUI enemyCount;
    //public EnemyCount enemyLeft;
    //public GameObject explostion;
    //private int goldCount;
    [SerializeField]
    PlayerStats PlayerFunc;
    [SerializeField]
    ThirdPersonMovement PlayerMove;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize () 
    {
        curState = FSMState.Spawn;
        curSpeed = 1.5f;
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

        if(!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        PlayerMove = GameObject.Find("Player/Main Character").GetComponent<ThirdPersonMovement>();
        PlayerFunc = GameObject.Find("Player/Main Character").GetComponent<PlayerStats>();

    }
    public void ActivateHitBox() //assign Tag to the part of the monster that has the collider
    {
        hitBox.GetComponent<Collider>().enabled = true;
    }

    public void DeactivateHitBox()
    {
        hitBox.GetComponent<Collider>().enabled = false;
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
            case FSMState.Spawn: Invoke(nameof(UpdateStareState), 4); break;
            case FSMState.Stare: UpdateStareState(); break;
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
    /// Stare state
    /// </summary>
    protected void UpdateStareState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with player
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= atkRange)
        {
            animator.SetBool("isChasing", false);
            curState = FSMState.Attack;
        }

        else
        {
            print("Switch to Manic Position");
            animator.SetBool("isChasing", true);
            curState = FSMState.Manic;
        }

        
        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
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

        //Go Forward
        transform.Translate(Time.deltaTime * curSpeed * Vector3.forward);

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
            curState = FSMState.Stare;
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
            curState = FSMState.Manic;
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
            curState = FSMState.Stare;
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
            //Explode();
        }
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if(collision.gameObject.CompareTag("Bullet") && PlayerMove.isAttacking == true)
        {
            animator.SetTrigger("wasHit");
            health -= PlayerFunc.ATT;
        }
        if(health <= 0)
        {
            PlayerFunc.GainEXP(50);
        }

    }


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
        /*float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }*/
        Destroy(gameObject, 1.5f);
    }

}
