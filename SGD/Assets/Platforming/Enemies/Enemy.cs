using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public abstract void Kill();
    public abstract void Die();
    public abstract void Activate();
}
