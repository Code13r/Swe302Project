using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using StarterAssets;

public class PlayerControl : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private ThirdPersonController thirdPersonController;

    [Header("Combat")]
    public Transform target;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float quickAttackDeltaDistance = 0.3f;
    [SerializeField] private float heavyAttackDeltaDistance = 0.1f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float airknockbackForce = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float reachTime = 0.2f;
    [SerializeField] private LayerMask enemyLayer;

    bool isAttacking = false;

    [Header("Debug")]
    [SerializeField] private bool debug;

    private EnemyBase oldTarget;
    private EnemyBase currentTarget;

    void Start()
    {
        if (TargetDetectionControl.instance.allTargetsInScene.Count > 0)
        {
            SetTarget(TargetDetectionControl.instance.allTargetsInScene[0]);
        }
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        if ((Vector3.Distance(transform.position, target.position) >= TargetDetectionControl.instance.detectionRange))
        {
            ClearTarget();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            Attack(0);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K))
        {
            Attack(1);
        }
    }

    public void Attack(int attackState)
    {
        if (isAttacking) return;

        thirdPersonController.canMove = false;
        TargetDetectionControl.instance.canChangeTarget = false;
        RandomAttackAnim(attackState);
    }

    private void RandomAttackAnim(int attackState)
    {
        switch (attackState)
        {
            case 0: QuickAttack(); break;
            case 1: HeavyAttack(); break;
        }
    }

    void QuickAttack()
    {
        int attackIndex = Random.Range(1, 4);
        if (debug) Debug.Log(attackIndex + " attack index");

        string animName = attackIndex switch
        {
            1 => "punch",
            2 => "kick",
            3 => "mmakick",
            _ => "punch"
        };

        PlayAttack(animName);
    }

    void HeavyAttack()
    {
        int attackIndex = Random.Range(1, 3);
        if (debug) Debug.Log(attackIndex + " attack index");

        string animName = attackIndex == 1 ? "heavyAttack1" : "heavyAttack2";
        PlayAttack(animName);
    }

    void PlayAttack(string animName)
    {
        anim.SetBool(animName, true);
        FaceTarget();
        isAttacking = true;
    }

    public void ResetAttack()
    {
        anim.SetBool("punch", false);
        anim.SetBool("kick", false);
        anim.SetBool("mmakick", false);
        anim.SetBool("heavyAttack1", false);
        anim.SetBool("heavyAttack2", false);
        thirdPersonController.canMove = true;
        TargetDetectionControl.instance.canChangeTarget = true;
        isAttacking = false;
    }

    public void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                knockbackDirection.y = airknockbackForce;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }

            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.SpawnHitVfx(enemy.transform.position);
            }

            healthBar enemyHealth = enemy.GetComponentInChildren<healthBar>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(20f);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (target != null && oldTarget != null)
        {
            oldTarget.ActiveTarget(false);
        }

        target = newTarget;
        currentTarget = newTarget.GetComponent<EnemyBase>();
        oldTarget = currentTarget;
        currentTarget.ActiveTarget(true);
    }

    public void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.ActiveTarget(false);
        }

        target = null;
        currentTarget = null;
        oldTarget = null;
    }

    void FaceTarget()
    {
        if (target == null) return;
        Vector3 direction = (target.position - transform.position);
        direction.y = 0;
        if (direction == Vector3.zero) return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.DORotateQuaternion(lookRotation, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackRange);
        }
    }
}
