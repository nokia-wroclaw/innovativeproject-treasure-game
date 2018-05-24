using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public abstract class Visibility : MonoBehaviour
{   
    public bool stunned = false;
    protected NavMeshAgent _agent;
    protected bool _chasing;

    public bool Chasing
    {
        get
        {
            return _chasing;
        }
        protected set
        {
            _chasing = value;

            SignalStateChange();
        }
    }    

    protected GameObject Player { get; private set; }


    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        _agent = GetComponent<NavMeshAgent>();

        InitComponents();

        StartCoroutine(PlayerVisibilityCheck());
    }

    protected abstract void InitComponents();   
    protected abstract IEnumerator PlayerVisibilityCheck(); 
	protected abstract void SignalStateChange();  


}

