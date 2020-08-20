using System.Collections;
using UnityEngine;

public class RangedUnitBehaviour : Enemy
{
    public GameObject projectile;
    public AudioSource impactSound;

    private void Start()
    {
        Activate();
    }

    // Update is called once per frame
    void Update()
    {

        /*shooter.transform.LookAt(target);
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        */
    }

    public override void Die()
    {
        StopAllCoroutines();
        impactSound.Play();
        base.Die();
    }
    IEnumerator BrainScope()
    {
        StartCoroutine(Attac());
        yield return new WaitForFixedUpdate();
    }
    IEnumerator Attac()
    {
        while (true)
        {
            /*
            GameObject bullet = Instantiate(projectile);//Instantiate(projectile, transform.position, Quaternion.identity);
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            bullet.transform.forward = shooter.transform.forward;
            */
            yield return new WaitForSeconds(2);
            
        }
        yield return new WaitForSeconds(3);
    }
}
