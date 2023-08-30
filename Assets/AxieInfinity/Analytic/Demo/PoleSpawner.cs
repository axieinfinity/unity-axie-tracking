using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Analytic.Demo
{
    public class PoleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject polePrefab;
        [SerializeField] private List<GameObject> poles;
        private float delayTime = 4.5f;
        private float time;

        private Bird bird;

        private void Awake()
        {
            bird = FindObjectOfType<Bird>();
        }

        private void Update()
        {
            if(bird!=null && !bird.isDeath)
            {
                poles.ForEach(x => x.transform.position -= Vector3.right * 1.5f * Time.deltaTime);
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    time = delayTime;
                    GameObject pole = Instantiate(polePrefab);
                    pole.transform.position = new Vector3(transform.position.x, Random.Range(4f, 10f));
                    poles.Add(pole);
                }
            }
        }
    }
}

