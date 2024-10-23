using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header ("Turret Type")]
    public TurretAI.TurretType type = TurretAI.TurretType.Single;
    public Transform target;

    [Header("Proyectile")]

    public float speed = 1;
    public float turnSpeed = 1;
    public bool catapult;
    public bool lockOn;

    [Header("Explosion")]
    public float recoil = 0.1f;
    public float explosionTimer = 1;
    public ParticleSystem explosion;

   


    

    private void Start()
    {
        if (catapult)
        {
            lockOn = true;
        }

        if (type == TurretAI.TurretType.Single)
        {
            Vector3 dir = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
   

    private void Update()
    {
        if (target == null)
        {
            Explosion();
            return;
        }

        if (transform.position.y < -0.2F)
        {
            Explosion();
        }

        explosionTimer -= Time.deltaTime;
        if (explosionTimer < 0)
        {
            Explosion();
        }

        if (type == TurretAI.TurretType.Catapult)
        {
            if (lockOn)
            {
                Vector3 Vo = CalculateCatapult(target.transform.position, transform.position, 1);

                transform.GetComponent<Rigidbody>().velocity = Vo;
                lockOn = false;
            }
        }
        else if(type == TurretAI.TurretType.Dual)
        {
            Vector3 dir = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * turnSpeed, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);

            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.LookRotation(newDirection);

        }
        else if (type == TurretAI.TurretType.Single)
        {
            float singleSpeed = speed * Time.deltaTime;
            transform.Translate(transform.forward * singleSpeed * 2, Space.World);
        }
    }

    Vector3 CalculateCatapult(Vector3 target, Vector3 origen, float time)
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
        Destroy(gameObject);
    }
}
