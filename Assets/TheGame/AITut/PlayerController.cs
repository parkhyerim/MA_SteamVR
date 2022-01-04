using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AITut
{
    public class PlayerController : MonoBehaviour
    {

        public float curHealth;
        public float maxHealth = 100;

        private void Awake()
        {
            curHealth = maxHealth;
        }
       
        public void GetDamage(float dmg)
        {
            curHealth -= dmg;
        }
    }
}

