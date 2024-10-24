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
    [SerializeField] private float shotCoolDown;


    public Vector3 randomRot;

  
    [Header ("referencias")]
    public Transform muzzleMain;
    public Transform muzzleSub;
    private Transform lockOnPos;
    public Transform turreyHead;
    private Transform lockOnPosition;


    public GameObject muzzleEff;
    public GameObject bullet;
    public GameObject currentTarget;

    public Animator animator;

    private bool shootLeft = true;
    private float nextTimeToShoot;




    void Start () {
        InvokeRepeating(nameof(CheckForTarget), 0, 0.5f);

        animator = transform.GetChild(0).GetComponent<Animator>();

        randomRot = new Vector3(0, Random.Range(0, 359), 0);
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

        ShootCheckTrigger();

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

    private void ShootCheckTrigger()
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
        Vector3 targetDirection = currentTarget.transform.position - turreyHead.position;
        targetDirection.y = 0;
       
        if (turretType == TurretType.Single)
        {
            turreyHead.forward = targetDirection;
        }
        else
        {
            turreyHead.transform.rotation = Quaternion.RotateTowards(turreyHead.rotation, Quaternion.LookRotation(targetDirection), loockSpeed * Time.deltaTime);
        }
    }

    private void ShootTrigger()
    {
       
        Shoot(currentTarget);
        
    }
    
   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDist);
    }

    public void IdleRitate()
    {
        bool refreshRandom = false;
        
        if (turreyHead.rotation != Quaternion.Euler(randomRot))
        {
            turreyHead.rotation = Quaternion.RotateTowards(turreyHead.transform.rotation, Quaternion.Euler(randomRot), loockSpeed * Time.deltaTime * 0.2f);
        }
        else
        {
            refreshRandom = true;

            if (refreshRandom)
            {

                int randomAngle = Random.Range(0, 359);
                randomRot = new Vector3(0, randomAngle, 0);
                refreshRandom = false;
            }
        }
    }

    public void Shoot(GameObject go)
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

    
