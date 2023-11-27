using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : Enemy
{
    public float moveSpeed = 1f;

    [SerializeField] protected float aggroDistance = 10f;
    private bool inAggroRange;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float timeBetweenNewPosition = 2f;

    private Vector3 spawnPoint;

    private Vector3 targetPosition;

    [SerializeField] private List<GameObject> slimes;
    [SerializeField] private List<GameObject> goblins;
    [SerializeField] private List<GameObject> skeletons;

    [SerializeField] private float timeToTransform = 2f;
    private bool isTransforming;

    [SerializeField] bool canSlime;
    [SerializeField] bool canGoblin;
    [SerializeField] bool canSkeleton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTargetPositionCoroutine());
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransforming || isDead)
            return;

        float distanceFromTarget = Vector3.Distance(transform.position, playerObject.transform.position);
        inAggroRange = distanceFromTarget <= aggroDistance;

        if (inAggroRange)
            StartCoroutine(TranformToEnemy());

        WanderAimlessly();
    }

    private void WanderAimlessly()
    {
        if(isDead) 
            return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep the y-axis movement to 0 to ensure sliding movement
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
    }

    private IEnumerator TranformToEnemy()
    {
        AudioManager.instance.Play("giggle");

        isTransforming = true;

        List<GameObject> enemiesToTransform = new List<GameObject>();

        // Put enemies from the lists into the array accordingly
        if (canSlime)
        {
            foreach (GameObject s in slimes)
                enemiesToTransform.Add(s);
        }
        if (canGoblin)
        {
            foreach (GameObject g in goblins)
                enemiesToTransform.Add(g);
        }
        if (canSkeleton)
        {
            foreach (GameObject sk in skeletons)
                enemiesToTransform.Add(sk);
        }

        // Check if there are any enemies to transform
        if (enemiesToTransform.Count > 0)
        {
            // Pick a random enemy from the list
            GameObject randomEnemy = enemiesToTransform[Random.Range(0, enemiesToTransform.Count)];

            // Check if the random enemy is part of the Slimes list
            if (slimes.Contains(randomEnemy))
                animator.SetTrigger("slime");
            // Check if the random enemy is part of the Goblins list
            else if (goblins.Contains(randomEnemy))
                animator.SetTrigger("goblin");
            // Check if the random enemy is part of the Skeletons list
            else if (skeletons.Contains(randomEnemy))
                animator.SetTrigger("skeleton");

            yield return new WaitForSeconds(timeToTransform);
            //create the transformed enemy
            Instantiate(randomEnemy, transform.position, Quaternion.LookRotation(Vector3.zero));

            //Destroy the real thing
            Destroy(gameObject);
        }
        else
        {
            // Handle the case when there are no enemies to transform
            Debug.LogWarning("No enemies to transform.");
            isTransforming = false;
        }
    }


    private IEnumerator UpdateTargetPositionCoroutine()
    {
        while (true)
        {
            // Update target position to a new random position within the bounds of the spawnpoint
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += spawnPoint;
            randomDirection.y = transform.position.y; // Assuming you want the companion to stay at its current y position

            targetPosition = randomDirection;

            // Wait for some time before changing the target position again
            yield return new WaitForSeconds(timeBetweenNewPosition);
        }
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //inflict damage
        if(!isTransforming)
            animator?.SetTrigger("pain");
        base.TakeDamage(damage, knockback, direction);
    }
}
