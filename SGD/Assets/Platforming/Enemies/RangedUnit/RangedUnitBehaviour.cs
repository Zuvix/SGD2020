using System.Collections;
using UnityEngine;

public class RangedUnitBehaviour : Enemy
{
    public GameObject projectile;
    public AudioSource impactSound;
    public GameObject ShootingPlace;
    public AudioSource castFrostBolt;
    public AudioSource createIce;
    GameObject bullet;
    GameObject target;
    public float rotSpeed = 0f;

    private void Start()
    {
        Activate();
        target = GameObject.FindGameObjectWithTag("Player");
    }


    public override void Die()
    {
        StopAllCoroutines();
        impactSound.Play();
        if (bullet != null)
        {
            bullet.GetComponent<Projectile>().Pop();
        }
        base.Die();
    }
    IEnumerator RotateTowardsPosition()
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
    IEnumerator BrainScope()
    {
        anim.speed = 1f;
        while (target!=null)
        {
            if ((transform.position - target.transform.position).magnitude < 15f)
            {
                FrostBolt();
                yield return new WaitForSeconds(8);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    void FrostBolt()
    {
            createIce.Play();
            Invoke("DelayedSound", 0.925f);
            StartCoroutine("RotateTowardsPosition");
            bullet = Instantiate(projectile,transform);
            anim.SetBool("isAttacking", true);
            bullet.transform.position = ShootingPlace.transform.position;
            //bullet.transform.forward = transform.forward;     
    }
    public void SendAttack()
    {
        if (isAlive&&bullet!=null)
        {
            StartCoroutine(bullet.GetComponent<Projectile>().FlyTowardsPlayer());
            bullet.transform.parent = null;
            StopCoroutine("RotateTowardsPosition");
        }
    }
    public void FinishAttack()
    {
        if(isAlive)
            anim.SetBool("isAttacking", false);
    }
    public void DelayedSound()
    {
        if(isAlive)
            castFrostBolt.Play();
    }
}
