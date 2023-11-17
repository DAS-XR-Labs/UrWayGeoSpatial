using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAS.Urway
{
    public class MoveAtRate : MonoBehaviour
    {


        public Vector3 endPosition;
        public float speed;
        private Vector3 startPosition;
        private bool fwd;
        public bool active;

        private void Start()
        {
            fwd = true;
            active = true;
            startPosition = transform.position;
        }

        public void ForwardMove()
        {
            //transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime * speed);
            transform.position = endPosition;
        }

        public void ReverseMove()
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * speed);
        }

        public void ToggleMove()
        {
            if (fwd)
            {
                fwd = false;
                ForwardMove();
            }
            else
            {
                fwd = true;
                ReverseMove();
            }
        }


        public void ToggleActive()
        {
            if (active)
            {
                active = false;
                gameObject.SetActive(true);
            }
            else
            {
                active = true;
                gameObject.SetActive(false);
            }
        }
    }
}
