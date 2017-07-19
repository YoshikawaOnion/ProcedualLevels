using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float WalkSpeed = 2;
    [SerializeField]
    private float JumpPower = 3;

    private Rigidbody2D rigidbody;

	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    private void FixedUpdate()
    {
        Vector3 accel = Vector3.zero;
		if (Input.GetKey(KeyCode.RightArrow))
		{
			accel += new Vector3(WalkSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			accel += new Vector3(-WalkSpeed, 0, 0);
		}
        rigidbody.AddForce(accel);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			rigidbody.AddForce(Vector2.up * JumpPower);
		}
    }
}
