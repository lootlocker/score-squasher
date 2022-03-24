using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyPrefab;

    public List<Enemy> enemies = new List<Enemy>();

    public int enemyCount;

    float spawnTimer;

    public float spawnInterval;

    public Transform player;

    public float movementSpeed;

    public PlayerController playerController;

    public GameObject enemyDieVFX;

    public Material regularEnemyMaterial;
    public Material enemyHitMaterial;

    public Transform spawnPosition;

    public GameObject scoreText;

    public TextMeshProUGUI scoreUIText;

    public AnimationCurve spawnCurve;

    public int score = 0;

    private Vector3 enemyMovementVector;
    private Vector3 enemyRotation = new Vector3(0, 50f, 0);

    void SpawnEnemies()
    {
        GameObject newEnemyObject = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity, transform);
        Enemy newEnemy = new Enemy(1f+movementSpeed, movementSpeed, newEnemyObject.transform);
        enemies.Add(newEnemy);
        StartCoroutine(EnemySpawnEffect(newEnemy.transform));
    }

    IEnumerator EnemySpawnEffect(Transform transformToAnimate)
    {
        float timer = 0f;
        float duration = 0.5f;
        transformToAnimate.localScale = Vector3.zero;
        while(timer <= duration)
        {
            transformToAnimate.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, spawnCurve.Evaluate(timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        transformToAnimate.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.gameState != GameController.GameState.Playing)
        {
            return;
        }
        movementSpeed += Time.deltaTime*0.05f;
        if(movementSpeed > 12f)
        {
            movementSpeed = 12f;
        }
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnInterval)
        {
            if (enemies.Count <= enemyCount)
            {
                SpawnEnemies();
                spawnTimer = 0f;
            }
        }
        MoveAndRotateEnemies();

        EnemyHealthCheck();
    }

    public Animator scoreAnimator;
    void EnemyHealthCheck()
    {
        List<Enemy> cleaning = new(enemies);
        foreach (var enemy in enemies)
        {
            foreach (var bullet in playerController.bullets)
            {
                if(Vector3.Distance(enemy.transform.position, bullet.position) <= 1f && bullet.gameObject.activeInHierarchy)
                {
                    enemy.health -= 1f;
                    StartCoroutine(ChangeMaterialRoutine(enemy.transform.GetComponent<MeshRenderer>()));
                    Instantiate(playerController.bulletDestroyVFX, bullet.position, Quaternion.identity);
                    Screenshake.instance.Shake(4f, 0.2f);
                    if (enemy.health <= 0f)
                    {
                        enemyCount+=3;
                        score += 100;
                        scoreUIText.text = score.ToString();
                        scoreAnimator.SetTrigger("Score");
                        Destroy(Instantiate(scoreText, enemy.transform.position, Quaternion.identity), 1f);
                        Screenshake.instance.Shake(8f, 0.1f);
                        Instantiate(enemyDieVFX, enemy.transform.position, Quaternion.identity);
                        Destroy(enemy.transform.gameObject);
                        cleaning.Remove(enemy);
                    }
                    bullet.gameObject.SetActive(false);
                }
            }
        }
        enemies = cleaning;
    }
    
    void MoveAndRotateEnemies()
    {
        foreach (var enemy in enemies)
        {

            enemy.transform.localEulerAngles += enemyRotation * Time.deltaTime;

            if (enemy != null)
            {
                enemyMovementVector = player.position - enemy.transform.position;
                enemyMovementVector = enemyMovementVector.normalized;      
            }
            foreach (var enemyNeighbour in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, enemyNeighbour.transform.position) <= 1f)
                {
                    Vector3 avoidance = enemy.transform.position - enemyNeighbour.transform.position;

                    enemyMovementVector += avoidance;
                }
            }
            
            enemy.transform.position += enemyMovementVector * Time.deltaTime * movementSpeed;
            // Outside clamping
            if (enemy.transform.position.x > 500f)
            {
                Vector3 newPos = enemy.transform.position;
                newPos.x = 500f;
                enemy.transform.position = newPos;
            }
            if (enemy.transform.position.x < -500f)
            {
                Vector3 newPos = enemy.transform.position;
                newPos.x = -500f;
                enemy.transform.position = newPos;
            }
            if (enemy.transform.position.z > 500f)
            {
                Vector3 newPos = enemy.transform.position;
                newPos.z = 500f;
                enemy.transform.position = newPos;
            }
            if (enemy.transform.position.z < -500f)
            {
                Vector3 newPos = enemy.transform.position;
                newPos.z = -500f;
                enemy.transform.position = newPos;
            }
        }
        
    }

    IEnumerator ChangeMaterialRoutine(MeshRenderer ren)
    {
        ren.material = enemyHitMaterial;
        yield return new WaitForSeconds(0.1f);
        if (ren != null)
        {
            ren.material = regularEnemyMaterial;
        }
    }

    
}

public class Enemy
{
    public float health;

    public float movementSpeed;

    public Transform transform;

    public Enemy(float health, float movementSpeed, Transform transform)
    {
        this.health = health;
        this.movementSpeed = movementSpeed;
        this.transform = transform;
    }
}
