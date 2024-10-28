using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public class TurretAI : MonoBehaviour {


    public enum TurretType
    {
        Single = 1,
        Dual = 2,
        Catapult = 3,
    }
    [Header("[Turret Type]")]
    [SerializeField] private TurretType turretType;


    [Header("Turret stats")]
    public float attackDist = 10.0f;
    public float attackDamage;
    public float shootCoolDown;
    private float timer;
    public float loockSpeed;
    private float nextTimeToShoot;
    [SerializeField] private float shotCoolDown;
    [SerializeField] private PoolObjectType bulletType;

    private Vector3 randomRotation;

  
    [Header ("referencias")]
    public Transform muzzleMain;
    public Transform muzzleSub;
    private Transform lockOnPos;
    public Transform turretHead;
    private Transform lockOnPosition;
    public GameObject muzzleEffect;
    public GameObject bullet;
    public GameObject currentTarget;
    public Animator animator;

    private bool shootLeft = true;
   




    void Start () 
    {
        InvokeRepeating(nameof(CheckForTarget), 0, 0.5f);

        if (turretHead.TryGetComponent<Animator>(out Animator component))
        {
            animator = component;
        }

        randomRotation = new Vector3(0, Random.Range(0, 359), 0);

        nextTimeToShoot = Time.time;
    }
	
	void Update () {
        if (currentTarget != null)
        {
            FollowTarget();

            if (DistanceToTarget() > attackDist)
            {
                currentTarget = null;
            }
        }
        else
        {
            IdleRitate();
        }

        Checktrigger();

        if (Input.GetMouseButtonDown(0))
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            ProjectileThrow(muzzleMain, player);
        }

        
    }

    float DistanceToTarget()
    {
        float currentTargetDist = Vector3.Distance(transform.position, currentTarget.transform.position);
        return currentTargetDist;

    }

   void Checktrigger()
    {
        if (currentTarget == null) return;

        if (nextTimeToShoot <= Time.time)
        {
            nextTimeToShoot = Time.time + shotCoolDown;

            Shoot(currentTarget);

            if (animator != null)
                animator.SetTrigger("Fire");
        }
    }

    private void CheckForTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackDist);
        float distanceAway = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.tag == "Player")
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < distanceAway)
                {
                    currentTarget = collider.gameObject;
                    distanceAway = distance;
                }
            }
        }
    }

    private void FollowTarget() 
    {
        Vector3 targetDirection = currentTarget.transform.position - turretHead.position;
        targetDirection.y = 0;
       
        if (turretType == TurretType.Single)
        {
            turretHead.forward = targetDirection;
        }
        else
        {
            turretHead.transform.rotation = Quaternion.RotateTowards(turretHead.rotation, Quaternion.LookRotation(targetDirection), loockSpeed * Time.deltaTime);
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDist);
    }

    public void IdleRitate()
    {
        if (turretHead.rotation != Quaternion.Euler(randomRotation))
        {
            turretHead.rotation = Quaternion.RotateTowards(turretHead.transform.rotation, Quaternion.Euler(randomRotation), loockSpeed * Time.deltaTime * 0.2f);
        }
        else
        {
            int randomAngle = Random.Range(0, 359);
            randomRotation = new Vector3(0, randomAngle, 0);
              
        }
    }

    public void Shoot(GameObject target)
    {
        if (turretType == TurretType.Catapult)
        {
            lockOnPosition = target.transform;
            ProjectileThrow(muzzleMain, lockOnPosition);
        }
        else if (turretType == TurretType.Dual)
        {
            if (shootLeft)
            {
                ProjectileThrow(muzzleMain, target.transform);
            }
            else
            {
                ProjectileThrow(muzzleSub, target.transform);
            }

            shootLeft = !shootLeft;
        }
        else
        {
            ProjectileThrow(muzzleMain, target.transform);
        }
    }

    private void ProjectileThrow(Transform shootPoint, Transform newTarget)
    {
        GameObject bullet = ObjectPoolingManager.Instance.GetPooledObject(bulletType);
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.Target = newTarget;

        bullet.SetActive(true);
        bullet.transform.position = shootPoint.position;

        if (!(turretType == TurretType.Catapult))
            projectile.SetRotation();

        Instantiate(muzzleEffect, muzzleMain.transform.position, muzzleMain.rotation);
    }
}

    
