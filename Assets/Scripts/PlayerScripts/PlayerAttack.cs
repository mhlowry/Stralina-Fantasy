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

    [SerializeField] private List<HitBox> hitBoxes;
    [SerializeField] private GameObject vfxObj;

    //collection of getter methods
    public string getAnim() {  return animTrigger; }
    public float getDuration() { return duration; }
    public float getImpact() { return impact; }
    public float getDelay() { return delay; }

    public GameObject getVfxObj() {  return vfxObj; }

    public List<HitBox> getHitBoxes() {  return hitBoxes; }
}

[System.Serializable]
public class HitBox
{
    [SerializeField] private Transform center;
    [SerializeField] private float size;

    public Transform getTransform() { return center; }

    public Vector3 getPosition() { return center.position; }
    public float getSize() { return size; }
}
