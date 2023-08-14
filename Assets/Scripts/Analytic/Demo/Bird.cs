using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Analytic.Demo
{
    public class Bird : MonoBehaviour
    {
        public bool isDeath;
        [SerializeField] private Rigidbody2D rb;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            FindObjectOfType<DemoScene>().Score += 1;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isDeath)
            {
                isDeath = true;
                FindObjectOfType<DemoScene>().GameOver();
            }
            
        }

        private void Update()
        {
            if (!isDeath)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
                {
                    rb.velocity = (Vector2.up * 3);
                }
            }
        }
    }
}

