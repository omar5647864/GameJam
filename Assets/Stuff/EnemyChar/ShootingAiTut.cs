
using UnityEngine;
using UnityEngine.AI;
public class ShootingAiTut : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public NavMeshAgent agent;
    public GameObject bulletSpawn;

    public Transform player;
    public GameObject gun;
    public Animator animator;

    //Stats
    public int health;

    //Check for Ground/Obstacles
    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    //Attack Player
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public bool isDead;
    public bool detectingDamage = false;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Special
   // public Material green, red, yellow;
    public GameObject projectile;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
    private void Update()
    {
        if (!isDead)
        {
            //Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            //Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (detectingDamage) ChasePlayer();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (isDead) return;

        if (!walkPointSet) SearchWalkPoint();

        //Calculate direction and walk to Point
        if (walkPointSet){
            agent.SetDestination(walkPoint);

            //Vector3 direction = walkPoint - transform.position;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15f);
        }

        //Calculates DistanceToWalkPoint
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

     //   GetComponent<MeshRenderer>().material = green;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //if (Physics.Raycast(walkPoint,-transform.up, 2,whatIsGround))
        walkPointSet = true;
    }
    private void ChasePlayer()
    {
        if (isDead) return;

        agent.SetDestination(player.position);
        animator.SetBool("isAttacking",false);
        animator.SetBool("isRunning",true);


        //GetComponent<MeshRenderer>().material = yellow;
    }
    private void AttackPlayer()
    {
        if (isDead) return;

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        //agent.Stop();
        animator.SetBool("isAttacking",true);
        animator.SetBool("isRunning",false);

        transform.LookAt(player);

        if (!alreadyAttacked){

            //Attack
            Rigidbody rb = Instantiate(projectile, bulletSpawn.transform.position, bulletSpawn.transform.rotation).GetComponent<Rigidbody>();

            //rb.velocity = Vector3.MoveTowards(transform.position, player.position, 32 * Time.deltaTime);

            rb.velocity = (player.position - rb.position).normalized * 50;

            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
             rb.AddForce(transform.up * 2, ForceMode.Impulse);

            //var dir = (player.transform.position - rb.transform.position).normalized;
            //rb.velocity = dir * 34;

            alreadyAttacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }

//        GetComponent<MeshRenderer>().material = red;
    }
    private void ResetAttack()
    {
        if (isDead) return;

        alreadyAttacked = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        detectingDamage = true;
        if (health < 0){
            isDead = true;
            animator.SetBool("isDead",true);
            audioSource.Stop();
            //Invoke("Destroyy", 2.8f);
        }
    }
    private void Destroyy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
