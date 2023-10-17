using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Item : MonoBehaviour
{
    [SerializeField, Range(1, 1000)] int rarity;
    public int GetRarity() {  return rarity; }

    private void OnTriggerEnter(Collider hitTarget)
    {
        if (hitTarget.gameObject.CompareTag("Player"))
            CollectItem(hitTarget.gameObject);
    }

    protected virtual void CollectItem(GameObject playerObj)
    {
        Debug.Log("You collected " + gameObject.name);
        Destroy(gameObject);
    }
}
