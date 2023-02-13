using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantAI : MonoBehaviour
{
    public Transform target;
    private Rigidbody rbody;
    public float health;
    public float maxHealth;
    private Text healText;
    private Image healBar;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float atkCooldownTimeMain;
    public float atkCooldownTime;
    public Animator animator;

    [SerializeField]
    ThirdPersonMovement PlayerMove;
    [SerializeField]
    PlayerStats PlayerFunc;
    // Start is called before the first frame update
    void Start()
    {
        healText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
        healBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
        rbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        healText.text = health.ToString();
        healBar.fillAmount = health / maxHealth;

        if(Vector3.Distance(transform.position, target.position) <= 20.0f)
        {
            transform.LookAt(target, Vector3.up);
            if (atkCooldownTime > 0)
            {
                atkCooldownTime -= Time.deltaTime;
            }

            else
            {
                animator.SetTrigger("Attacking");
                atkCooldownTime = atkCooldownTimeMain;
            }
        }
        if(health <= 0)
        {
            health = 0;
            animator.SetBool("isDead", true);
            Destroy(gameObject, 1.5f);
        }
            
    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 20;
        Destroy(bullet, 2.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.CompareTag("Bullet") && PlayerMove.isAttacking == true)
        {
            animator.SetTrigger("wasHit");
            health -= PlayerFunc.ATT;
        }
        if(health <= 0)
        {
            PlayerFunc.GainEXP(30);
        }
    }
}
