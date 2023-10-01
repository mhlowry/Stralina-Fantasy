using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/* duration is how long the attack will last
 * impact is the momentum the attack carries
 * vfxIndex corresponds to the VFX animation that will get called by VFX controller
 */

[System.Serializable]
public class PlayerAttack
{
    //This is the visual stuff
    [SerializeField] private string animTrigger;

    //This is stuff that affects game states
    [SerializeField] private float duration;
    [SerializeField] private float impact;
    [SerializeField] private float delay;
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;

    [SerializeField] private List<HitBox> hitBoxes;
    [SerializeField] private GameObject vfxObj;

    //collection of getter methods
    public string GetAnim() {  return animTrigger; }
    public float GetDuration() { return duration; }
    public float GetImpact() { return impact; }
    public float GetDelay() { return delay; }
    public float GetDamage() { return damage; }
    public float GetKnockBack() {  return knockBack; }

    public GameObject GetVfxObj() {  return vfxObj; }

    public List<HitBox> GetHitBoxes() {  return hitBoxes; }
}

[System.Serializable]
public class HitBox
{
    [SerializeField] private Transform center;
    [SerializeField] private float size;

    public Transform GetTransform() { return center; }

    public Vector3 GetPosition() { return center.position; }
    public float GetSize() { return size; }
}
