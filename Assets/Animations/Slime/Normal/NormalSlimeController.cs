using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSlimeController : MonoBehaviour
{
    private Animator _animator;
    private bool _isAggro;
    private Slime _slime;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _isAggro = false;
        _slime = GetComponent<Slime>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_slime != null && _slime.playerDistance() <= _slime.aggroDistance && !_isAggro) 
            _animator.SetBool("isAggro", true);
    }
}
