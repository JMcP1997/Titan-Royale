using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FSM : MonoBehaviour
{

    protected virtual void Initialize() { }

    protected virtual void FSMUpdate() { }

    protected virtual void FSMFixedUpdate() { }


    void Start()
    {
        Initialize();
    }

    private protected virtual void Update()
    {
        FSMUpdate();
    }

    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }


}
