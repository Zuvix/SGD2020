using System.Collections;
using UnityEngine;

public class RangedUnitBehaviour : Enemy
{
    public GameObject projectile;
    public AudioSource impactSound;
    public GameObject ShootingPlace;
    public GameObject fireBall;
    public AudioSource castFrostBolt;
    public AudioSource createIce;
    public AudioSource castFireBolt;
    public Light wandLight;
    GameObject bullet;
    GameObject target;
    public float rotSpeed = 0f;

    private void Start()
    {
        wandLight = GetComponentInChildren<Light>();
        Activate();
        target = GameObject.FindGameObjectWithTag("Player");
    }
    //Vojto si super

    public override void Die()
    {
        StopAllCoroutines();
        impactSound.Play();
        LevelManager.Instance.StartCoroutine(LevelManager.Instance.SpawnMonster("w", startPos, startRot));
        if (bullet != null)
        {
            bullet.GetComponent<Projectile>().Pop();
        }
        base.Die();
    }
    IEnumerator RotateTowardsPosition()
    {
        if (target != null)
        {
            Vector3 tarPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            Vector3 targetDirection = tarPos - transform.position;
            while (true)
            {
                Quaternion targetRotaion = Quaternion.LookRotation(tarPos - transform.position);
                if (Quaternion.Angle(transform.rotation, targetRotaion) > 1f)
                {
                    float singleStep = rotSpeed * Time.deltaTime;
                    Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDirection);
                }
                yield return new WaitForFixedUpdate();
            }
        }
        yield return new WaitForFixedUpdate();
    }
    IEnumerator BrainScope()
    {
        anim.speed = 1f;
        while (target!=null && isAlive)
        {
            if ((transform.position - target.transform.position).magnitude < 15f)
            {
                float random = Random.Range(0f, 100f);
                if (random <= 72.5f)
                {
                    FrostBolt();
                    yield return new WaitForSeconds(5);
                }
                else
                {
                    yield return MeteorShower();
                    yield return new WaitForSeconds(2.5f);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    void FrostBolt()
    {
            wandLight.color = new Color(20, 155, 197) * 0.00390625f;
            wandLight.intensity = 0.3f;
            createIce.Play();
            Invoke("DelayedSound", 0.925f);
            StartCoroutine("RotateTowardsPosition");
            bullet = Instantiate(projectile,transform);
            bullet.GetComponent<Projectile>().owner = this.gameObject;
            anim.SetBool("isAttacking", true);
            bullet.transform.position = ShootingPlace.transform.position;
            //bullet.transform.forward = transform.forward;     
    }
    IEnumerator MeteorShower()
    {
        float timeCasted=0f;
        anim.SetBool("isSummoning", true);
        StartCoroutine("RotateTowardsPosition");
        yield return new WaitForSeconds(0.4f);
        StopCoroutine("RotateTowardsPosition");
        wandLight.color = new Color(144, 48, 33) * 0.00390625f;
        wandLight.intensity = 0.4f;
        castFireBolt.Play();
        while (timeCasted < 5f && isAlive)
        {
            int i = 0;
            Vector3 groundTerget = Vector3.zero;
            while(i<5 && groundTerget.Equals(Vector3.zero))
            {
                groundTerget = CheckPositionForGround(target.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0.5f, Random.Range(-1.5f, 1.5f)));
                i++;
                yield return new WaitForFixedUpdate();
            }
            if (groundTerget != Vector3.zero)
            {
                GameObject g = Instantiate(fireBall);
                g.transform.position = groundTerget+Vector3.up*0.05f;
                yield return new WaitForSeconds(0.75f);
                timeCasted += 0.75f;
            }
            StartCoroutine("RotateTowardsPosition");
            yield return new WaitForSeconds(0.25f);
            timeCasted += 0.25f;
            StopCoroutine("RotateTowardsPosition");

        }
        anim.SetBool("isSummoning", false);
        wandLight.color = new Color(53, 60, 91) * 0.00390625f;
        wandLight.intensity = 0.2f;
    }
    public Vector3 CheckPositionForGround(Vector3 position)
    {
        RaycastHit hit;
        LayerMask lm = LayerMask.GetMask("Ground", "LivingGround");
        Ray ray = new Ray(position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 10f, lm))
        {
            return hit.point;
        }
        else return Vector3.zero;     
    }
    public void SendAttack()
    {
        if (isAlive&&bullet!=null)
        {
            // StartCoroutine(bullet.GetComponent<Projectile>().FlyTowardsPlayer());
            StartCoroutine(bullet.GetComponent<Projectile>().FlyTowardsPoint(transform.position));
            bullet.transform.parent = null;
            StopCoroutine("RotateTowardsPosition");
        }
    }
    public void FinishAttack()
    {
        if (isAlive)
        {
            anim.SetBool("isAttacking", false);
            wandLight.color = new Color(53, 60, 91)*0.00390625f;
            wandLight.intensity = 0.2f;
        }
    }
    public void DelayedSound()
    {
        if(isAlive)
            castFrostBolt.Play();
    }
}
