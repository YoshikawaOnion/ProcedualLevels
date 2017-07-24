using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedualLevels.Models;
using ProcedualLevels.Common;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField]
        private float walkSpeed;

        private Hero Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }

        public void Initialize(Hero hero)
        {
            Hero = hero;
        }

        // Use this for initialization
        void Start()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
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
            Rigidbody.velocity = Rigidbody.velocity.MergeX(velocity);
        }
    }
}