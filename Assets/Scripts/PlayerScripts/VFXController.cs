using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [SerializeField] private List<PlayerVFX> attackVFX;

    // Start is called before the first frame update
    void Awake()
    {
        DisableAttackVFX();
    }

    //Getter method for ComboController
    public void playAttackVFX(int attackIndex)
    {
        StartCoroutine(activateAttackVFX(attackIndex));
    }

    private IEnumerator activateAttackVFX(int attackIndex)
    {
        yield return new WaitForSeconds(attackVFX[attackIndex].delay);
        attackVFX[attackIndex].vfxObj.transform.rotation = transform.rotation;
        attackVFX[attackIndex].vfxObj.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        DisableAttackVFX(attackIndex);
    }

    //this disables all VFX objects
    public void DisableAttackVFX()
    {
        for (int i = 0; i < attackVFX.Count; i++)
        {
            attackVFX[i].vfxObj.SetActive(false);
        }
    }
    //this disables one VFX object
    private void DisableAttackVFX(int vfxToDisable)
    {
        attackVFX[vfxToDisable].vfxObj.SetActive(false);
    }

    [System.Serializable]
    public class PlayerVFX
    {
        public GameObject vfxObj;
        public float delay;
    }
}
