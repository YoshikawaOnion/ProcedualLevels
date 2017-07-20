﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float walkSpeed = 2;
        [SerializeField]
        private float jumpPower = 3;
        [SerializeField]
        private int jumpCopyCount = 5;
        [SerializeField]
        private float copyOffset = -8;
        [SerializeField]
        private float jetSpeed = 10;

        private new Rigidbody2D rigidbody;
        private GameObject copyPrefab;

        // Use this for initialization
        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            copyPrefab = Resources.Load<GameObject>("Prefabs/Character/Copy");
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            float velocity = 0;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocity += walkSpeed;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocity -= walkSpeed;
            }
            rigidbody.velocity = rigidbody.velocity.MergeX(velocity);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody.AddForce(Vector2.up * jumpPower);
                for (int i = 0; i < jumpCopyCount; i++)
                {
                    var obj = Instantiate(copyPrefab);
                    obj.transform.position = transform.position.AddY(copyOffset);
                    var rigid = obj.GetComponent<Rigidbody2D>();
                    var angle = Helper.RandomInRange(-60, 60) + 180;
                    var v = Vector2Extensions.FromAngleLength(angle, jetSpeed);
                    rigid.velocity = v;
                }
            }
        }
    }
}