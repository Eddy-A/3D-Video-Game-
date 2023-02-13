using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM : MonoBehaviour 
{
    // Enemy GameObject
    [SerializeField]
    protected GameObject Enemy;

    [SerializeField]
    protected GameObject Player;

    //Player Transform
    protected Transform playerTransform;

    //List of points for patrolling
    protected List<Vector3> pointList = new List<Vector3>();

    //Next destination position of the NPC Tank
    protected Vector3 destPos;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

	// Use this for initialization
	void Start () 
    {
        pointList.Insert(0, Enemy.transform.position + new Vector3(0, 0.5f, 10));
        pointList.Insert(1, Enemy.transform.position - new Vector3(0, -0.5f, 10));
        Initialize();
    }
	
	// Update is called once per frame
	void Update () 
    {
        FSMUpdate();
	}

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }    
}
