using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDangerDummy : DangerDummy
{
    // Start is called before the first frame update
    void Start()
    {
        regularColor = new Color(0.6f, 0.6f, 1.0f);
    }
    // Update is called once per frame
    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        StartCoroutine(CallTakeDamage());
    }

    IEnumerator CallTakeDamage()
    {
        yield return new WaitForSeconds(2);
        base.TakeDamage(0, 0, new Vector3(1,1,1));
    }
}
