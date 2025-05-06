
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectionControl : MonoBehaviour
{
    int currentTargetIndex = 0;

    public void CycleToNextTarget()
    {
        if (allTargetsInScene.Count == 0) return;

        List<Transform> validTargets = new List<Transform>();
        foreach (Transform enemy in allTargetsInScene)
        {
            if (Vector3.Distance(transform.position, enemy.position) <= detectionRange)
            {
                validTargets.Add(enemy);
            }
        }

        if (validTargets.Count == 0) return;

        currentTargetIndex++;
        if (currentTargetIndex >= validTargets.Count)
        {
            currentTargetIndex = 0;
        }

        playerControl.SetTarget(validTargets[currentTargetIndex]);
    }

    public static TargetDetectionControl instance;

    [Header("Components")]
    public PlayerControl playerControl;

    [Header("Scene")]
    public List<Transform> allTargetsInScene = new List<Transform>();

    [Space]
    [Header("Target Detection")]
    public LayerMask whatIsEnemy;
    public bool canChangeTarget = true;

    [Tooltip("Detection Range: \n Player range for detecting potential targets.")]
    [Range(0f, 15f)] public float detectionRange = 10f;

    [Tooltip("Dot Product Threshold \nHigher Values: More strict alignment required \nLower Values: Allows for broader targeting")]
    [Range(0f, 1f)] public float dotProductThreshold = 0.15f;

    [Space]
    [Header("Debug")]
    public bool debug;
    public Transform checkPos;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PopulateTargetInScene();
        StartCoroutine(RunEveryXms());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (canChangeTarget)
            {
                CycleToNextTarget();
            }
        }
    }

    private void PopulateTargetInScene()
    {
        EnemyBase[] allGameObjects = FindObjectsOfType<EnemyBase>();

        List<EnemyBase> gameObjectList = new List<EnemyBase>(allGameObjects);

        if (debug)
            Debug.Log("Number of targets found: " + gameObjectList.Count);

        foreach (EnemyBase obj in gameObjectList)
        {
            allTargetsInScene.Add(obj.transform);
        }
    }

    private IEnumerator RunEveryXms()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);
            GetEnemyInInputDirection();
        }
    }

    public void GetEnemyInInputDirection()
    {
        if (canChangeTarget)
        {
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (inputDirection != Vector3.zero)
            {
                inputDirection = Camera.main.transform.TransformDirection(inputDirection);
                inputDirection.y = 0;
                inputDirection.Normalize();

                Transform closestEnemy = GetClosestEnemyInDirection(inputDirection);

                if (closestEnemy != null && (Vector3.Distance(transform.position, closestEnemy.position)) <= detectionRange)
                {
                    playerControl.SetTarget(closestEnemy);
                    if (debug)
                        Debug.Log("Closest enemy in direction: " + closestEnemy.name);
                }
            }
        }
    }

    Transform GetClosestEnemyInDirection(Vector3 inputDirection)
    {
        Transform closestEnemy = null;
        float maxDotProduct = dotProductThreshold;

        for (int i = allTargetsInScene.Count - 1; i >= 0; i--)
        {
            Transform enemy = allTargetsInScene[i];

            if (enemy == null)
            {
                allTargetsInScene.RemoveAt(i);
                continue;
            }

            Vector3 enemyDirection = (enemy.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(inputDirection, enemyDirection);

            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
