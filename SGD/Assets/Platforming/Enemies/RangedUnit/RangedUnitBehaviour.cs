using System.Collections;
using UnityEngine;

public class RangedUnitBehaviour : Enemy
{
    public Transform target;
    public GameObject projectile;
    public GameObject shooter;

    private void Start()
    {
        Activate();
    }
    public override void Activate()
    {
        StartCoroutine(BrainScope());
        Debug.Log("Ranged Unit Activated");
    }

    // Update is called once per frame
    void Update()
    {
        shooter.transform.LookAt(target);
        transform.LookAt(new Vector3(target.position.x,transform.position.y, target.position.z));
    }


    public override void Kill()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);  
        Debug.Log("Ranged Unit killed, rip");
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
            GameObject bullet = Instantiate(projectile);//Instantiate(projectile, transform.position, Quaternion.identity);
            bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y - 0.2f, bullet.transform.position.z);
            bullet.transform.forward = shooter.transform.forward;
            yield return new WaitForSeconds(2);
        }
        yield return new WaitForSeconds(3); 
    }
}
