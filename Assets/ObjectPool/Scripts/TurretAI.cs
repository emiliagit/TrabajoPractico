using UnityEngine;



public class TurretAI : MonoBehaviour {


    public enum TurretType
    {
        Single = 1,
        Dual = 2,
        Catapult = 3,
    }


    public GameObject currentTarget;
    public Transform turreyHead;

    public float attackDist = 10.0f;
    public float attackDamage;
    public float shootCoolDown;
    private float timer;
    public float loockSpeed;

    //public Quaternion randomRot;
    public Vector3 randomRot;
    public Animator animator;

    [Header("[Turret Type]")]
    [SerializeField] private TurretType turretType;

    public Transform muzzleMain;
    public Transform muzzleSub;
    public GameObject muzzleEff;
    public GameObject bullet;
    private bool shootLeft = true;

    private Transform lockOnPos;


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

        timer += Time.deltaTime;
        if (timer >= shootCoolDown)
        {
            if (currentTarget != null)
            {
                timer = 0;
                
                if (animator != null)
                {
                    animator.SetTrigger("Fire");
                    ShootTrigger();
                }
                else
                {
                    ShootTrigger();
                }
            }
        }
	}

    float DistanceToTarget()
    {
        float currentTargetDist = Vector3.Distance(transform.position, currentTarget.transform.position);
        return currentTargetDist;

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
            lockOnPos = go.transform;
            //Aplicar POOL OBJECT
            Instantiate(muzzleEff, muzzleMain.transform.position, muzzleMain.rotation);
            GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
            Projectile projectile = missleGo.GetComponent<Projectile>();
            projectile.target = lockOnPos;
        }
        else if(turretType == TurretType.Dual)
        {
            if (shootLeft)
            {
                //Aplicar POOL OBJECT
                Instantiate(muzzleEff, muzzleMain.transform.position, muzzleMain.rotation);
                GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
                Projectile projectile = missleGo.GetComponent<Projectile>();
                projectile.target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }
            else
            {
                //Aplicar POOL OBJECT
                Instantiate(muzzleEff, muzzleSub.transform.position, muzzleSub.rotation);
                GameObject missleGo = Instantiate(bullet, muzzleSub.transform.position, muzzleSub.rotation);
                Projectile projectile = missleGo.GetComponent<Projectile>();
                projectile.target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }

            shootLeft = !shootLeft;
        }
        else
        {
            //Aplicar POOL OBJECT
            Instantiate(muzzleEff, muzzleMain.transform.position, muzzleMain.rotation);
            GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
            Projectile projectile = missleGo.GetComponent<Projectile>();
            projectile.target = currentTarget.transform;
        }
    }
}
