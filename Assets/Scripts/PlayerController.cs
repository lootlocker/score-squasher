using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Leaderboard leaderboard;

    private Rigidbody rb;

    Vector3 movement;

    public float movementSpeed;

    public Transform visuals;

    public float rotLerpSpeed;

    Vector3 previousMovement;

    public GameObject bulletPrefab;

    float shootingTimer;

    public float shootingInterval;

    public float bulletForce;
    public float bulletDestroyTime;

    public EnemyController enemyController;

    public float shootingRadius;

    public List<Transform> bullets = new List<Transform>();

    public List<Transform> bulletsToDestroy = new List<Transform>();

    int score;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.gameState != GameController.GameState.Playing)
        {
            return;
        }
        score = enemyController.score;
        shootingTimer += Time.deltaTime;
        if(shootingTimer >= shootingInterval)
        {
            Shoot();
            shootingTimer = 0f;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;
        if(movement.magnitude != 0)
        {
            previousMovement = movement;
        }
        visuals.rotation = Quaternion.Slerp(visuals.rotation, Quaternion.LookRotation(transform.position - (transform.position + previousMovement)), Time.deltaTime*rotLerpSpeed);

        
    }


    private void Shoot()
    {
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;
        foreach (var enemy in enemyController.enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= 1f)
                {
                    StartCoroutine(DieRoutine());
                }
                if (distance < closestDistance && distance <= shootingRadius)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        if (closestEnemy != null)
        {
            Vector3 rotationToTarget = closestEnemy.position - transform.position;
            rotationToTarget = rotationToTarget.normalized;
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(rotationToTarget));
            newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * bulletForce, ForceMode.Impulse);
            bullets.Add(newBullet.transform);
            StartCoroutine(RemoveBulletRoutine(newBullet.transform));
        }
    }

    private void FixedUpdate()
    {
        if (GameController.instance.gameState != GameController.GameState.Playing)
        {
            return;
        }
        rb.position += movement * movementSpeed*Time.fixedDeltaTime;

        // Outside clamping
        if (rb.position.x > 500f)
        {
            Vector3 newPos = rb.transform.position;
            newPos.x = 500f;
            rb.transform.position = newPos;
        }
        if (rb.transform.position.x < -500f)
        {
            Vector3 newPos = rb.transform.position;
            newPos.x = -500f;
            rb.transform.position = newPos;
        }
        if (rb.transform.position.z > 500f)
        {
            Vector3 newPos = rb.transform.position;
            newPos.z = 500f;
            rb.transform.position = newPos;
        }
        if (rb.transform.position.z < -500f)
        {
            Vector3 newPos = rb.transform.position;
            newPos.z = -500f;
            rb.transform.position = newPos;
        }
    }

    public GameObject bulletDestroyVFX;
    IEnumerator RemoveBulletRoutine(Transform bulletTransform)
    {
        yield return new WaitForSeconds(bulletDestroyTime);
        bullets.Remove(bulletTransform);
        bulletTransform.gameObject.SetActive(false);
        
        Destroy(bulletTransform.gameObject);
    }

    IEnumerator DieRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        yield return leaderboard.SubmitScoreRoutine(score);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
