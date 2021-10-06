using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    public Vector3 moveDir;
    public bool fired = false;

    private float lifespan = 5;

    private void Update()
    {
        if(fired)
        {
            transform.position += moveDir * 7 * Time.deltaTime;
            lifespan -= Time.deltaTime;
            if(lifespan <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
