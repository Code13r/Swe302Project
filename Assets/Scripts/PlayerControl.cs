using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using StarterAssets;
public class PlayerControl : MonoBehaviour
{
    [Space]
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private ThirdPersonController thirdPersonController;
   // [SerializeField] private GameControl gameControl;
 
    [Space]
    [Header("Combat")]
    public Transform target;
    [SerializeField] private Transform attackPos;
    [Tooltip("Offset Stoping Distance")][SerializeField] private float quickAttackDeltaDistance;
    [Tooltip("Offset Stoping Distance")][SerializeField] private float heavyAttackDeltaDistance;
    [SerializeField] private float knockbackForce = 10f; 
    [SerializeField] private float airknockbackForce = 10f; 
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float reachTime = 0.3f;
    [SerializeField] private LayerMask enemyLayer;
    bool isAttacking = false;

    [Space]
    [Header("Debug")]
    [SerializeField] private bool debug;

    // Start is called before the first frame update
  void Start()
{
    // TEMP: Auto-assign first target in the scene
    if (TargetDetectionControl.instance.allTargetsInScene.Count > 0)
    {
        ChangeTarget(TargetDetectionControl.instance.allTargetsInScene[0]);
    }
}

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        if((Vector3.Distance(transform.position, target.position) >= TargetDetectionControl.instance.detectionRange))
        {
            NoTarget();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack(0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Attack(1);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack(1);
        }

    }

    #region Attack, PerformAttack, Reset Attack, Change Target
  

    public void Attack(int attackState)
    {
        if (isAttacking)
        {
            return;
        }

        thirdPersonController.canMove = false;
        TargetDetectionControl.instance.canChangeTarget = false;
        RandomAttackAnim(attackState);
       
    }

    private void RandomAttackAnim(int attackState)
    {
        

        switch (attackState) 
        {
            case 0: //Quick Attack

                QuickAttack();
                break;

            case 1:
                HeavyAttack();
                break;

        }


       
    }

   void QuickAttack()
{
    int attackIndex = Random.Range(1, 4);
    if (debug)
    {
        Debug.Log(attackIndex + " attack index");
    }

    string animName = "";

    switch (attackIndex)
    {
        case 1: animName = "punch"; break;
        case 2: animName = "kick"; break;
        case 3: animName = "mmakick"; break;
    }

    Vector3 attackDirection = target != null ? target.position : transform.position + transform.forward * 1.5f;
    MoveTowardsTarget(attackDirection, quickAttackDeltaDistance, animName);
    isAttacking = true;
}

   void HeavyAttack()
{
    int attackIndex = Random.Range(1, 3);
    if (debug)
    {
        Debug.Log(attackIndex + " attack index");
    }

    string animName = attackIndex == 1 ? "heavyAttack1" : "heavyAttack2";

    Vector3 attackDirection = target != null ? target.position : transform.position + transform.forward * 1.5f;
    FaceThis(attackDirection);
    anim.SetBool(animName, true);
    isAttacking = true;
}


    public void ResetAttack() // Animation Event ---- for Reset Attack
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
    // Detect enemies in attack range
    Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, enemyLayer);

    foreach (Collider enemy in hitEnemies)
    {
        // Knockback (optional)
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            Vector3 knockbackDirection = enemy.transform.position - transform.position;
            knockbackDirection.y = airknockbackForce;
            enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
        }

        // Spawn hit effect (optional visual)
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.SpawnHitVfx(enemy.transform.position);
        }

        // Apply damage using health bar script
        healthBar enemyHealth = enemy.GetComponentInChildren<healthBar>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(20f); // Adjust damage as needed
        }
    }
}



    private EnemyBase oldTarget;
    private EnemyBase currentTarget;
    public void ChangeTarget(Transform target_)
    {
        
        if(target != null)
        {
            //oldTarget = target_.GetComponent<EnemyBase>(); //clear old target
            oldTarget.ActiveTarget(false);
        }
       
        target = target_;

        oldTarget = target_.GetComponent<EnemyBase>(); //set current target
        currentTarget = target_.GetComponent<EnemyBase>();
        currentTarget.ActiveTarget(true);

    }

    private void NoTarget() // When player gets out of range of current Target
    {
        currentTarget.ActiveTarget(false);
        currentTarget = null;
        oldTarget = null;
        target = null;
    }

    #endregion


    #region MoveTowards, Target Offset and FaceThis
    public void MoveTowardsTarget(Vector3 target_, float deltaDistance, string animationName_)
    {

        PerformAttackAnimation(animationName_);
        FaceThis(target_);
        Vector3 finalPos = TargetOffset(target_, deltaDistance);
        finalPos.y = 0;
        transform.DOMove(finalPos, reachTime);

    }

    public void GetClose()
{
    Vector3 getCloseTarget;

    if (target != null)
    {
        getCloseTarget = target.position;
    }
    else if (oldTarget != null)
    {
        getCloseTarget = oldTarget.transform.position;
    }
    else
    {
        // Fallback: attack forward if no targets exist
        getCloseTarget = transform.position + transform.forward * 1.5f;
    }

    FaceThis(getCloseTarget);
    Vector3 finalPos = TargetOffset(getCloseTarget, 1.4f);
    finalPos.y = 0;
    transform.DOMove(finalPos, 0.2f);
}


    void PerformAttackAnimation(string animationName_)
    {
        anim.SetBool(animationName_, true);
    }

    public Vector3 TargetOffset(Vector3 target, float deltaDistance)
    {
        Vector3 position;
        position = target;
        return Vector3.MoveTowards(position, transform.position, deltaDistance);
    }

    public void FaceThis(Vector3 target)
    {
        Vector3 target_ = new Vector3(target.x, target.y, target.z);
        Quaternion lookAtRotation = Quaternion.LookRotation(target_ - transform.position);
        lookAtRotation.x = 0;
        lookAtRotation.z = 0;
        transform.DOLocalRotateQuaternion(lookAtRotation, 0.2f);
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange); // Visualize the attack range
    }
}
