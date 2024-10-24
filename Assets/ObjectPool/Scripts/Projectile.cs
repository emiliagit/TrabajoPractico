using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Turret Type")]
    [SerializeField] private TurretAI.TurretType turretType;

    [Header("Proyectile")]
    public Transform target;
    public Transform Target { get => target; set => target = value; }
    [SerializeField] private PoolObjectType objectType;

    public float shootSpeed;
    public float turnSpeed;
    public bool catapult;
    public bool lockOn;

    [Header("Explosion")]
    public float recoil;
    public float explosionTimer;
    public ParticleSystem explosion;


    private void OnEnable()
    {
        if (catapult)
        {
            lockOn = true;
        }

        if (turretType == TurretAI.TurretType.Single)
        {
            transform.rotation = Quaternion.LookRotation(SetDirection());
        }
    }
    


    private void Update()
    {
        explosionTimer -= Time.deltaTime;
        CheckForExplosion();

        HandleCatapult();
        HandleDual();
        HandleSingle();

    }

    void CheckForExplosion()
    {
        if (target == null || transform.position.y < -0.2F || explosionTimer < 0)
        {
            Explosion();
            return;
        }
    }

    void HandleCatapult()
    {
        if (turretType == TurretAI.TurretType.Catapult)
        {
            if (lockOn)
            {
                Vector3 velocity = CalculateCatapultSpped(target.transform.position, transform.position, 1);
                transform.GetComponent<Rigidbody>().velocity = velocity;
                lockOn = false;
            }
        }
        
    }

    void HandleDual()
    {
        if (turretType == TurretAI.TurretType.Dual)
        {
            Vector3 dir = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * turnSpeed, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);
            transform.Translate(Vector3.forward * Time.deltaTime * shootSpeed);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void HandleSingle()
    {
        if (turretType == TurretAI.TurretType.Single)
        {
            float singleSpeed = shootSpeed * Time.deltaTime;
            transform.Translate(transform.forward * singleSpeed * 2, Space.World);
        }
    }


    private Vector3 SetDirection()
    {
        if (target == null)
            return Vector3.zero;

        Vector3 newDirection = target.position - transform.position;

        return newDirection.normalized;
    }
    public void SetRotation()
    {
        transform.rotation = Quaternion.LookRotation(SetDirection());
    }



    Vector3 CalculateCatapultSpped(Vector3 target, Vector3 origen, float time)
    {
        Vector3 distanceToTarget = target - origen;
        distanceToTarget.y = 0;

        float horizontalSpeed = distanceToTarget.magnitude / time;
        float yVelocity = distanceToTarget.y / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceToTarget.normalized;

        result *= horizontalSpeed;
        result.y = yVelocity;

        return result;
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player")) return;
        {
            PlayerRecoil(other.transform);
            Explosion();
        }
    }

    private void PlayerRecoil (Transform player)
    {
        Vector3 recoilDirection = player.position - transform.position;
        Vector3 playerPosition = player.position + (recoilDirection.normalized * recoil);
        playerPosition.y = 1;
        player.position = playerPosition;
       
    }

    public void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        ObjectPoolingManager.Instance.CoolObject(gameObject, objectType);
    }
}
