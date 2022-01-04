using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AITut
{

    public enum AI_STATE
    {
        IDLE,
        CHASING,
        ATTACKING
    }
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour
    {
        [Header("Settings")]
        public AI_STATE state = AI_STATE.IDLE;
        public float atkRange;
        public float dmg = 5;
        public float aggroRange = 7;

        public PlayerController target;

        private NavMeshAgent agent;


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(Vector3.Distance(transform.position, target.transform.position) <= aggroRange)
            {
                // Follow Player
                agent.SetDestination(target.transform.position);

                if (agent.hasPath)
                {
                    if (agent.remainingDistance <= atkRange)
                    {
                        // Debug.Log("Should Attack");
                        state = AI_STATE.ATTACKING;
                    }
                    else
                    {
                        state = AI_STATE.CHASING;
                    }

                }
            }
            else
            {
                state = AI_STATE.IDLE;
            }
           

            switch (state)
            {
                case AI_STATE.ATTACKING:
                    target.GetDamage(dmg * Time.deltaTime);
                    return;

                case AI_STATE.IDLE:
                    // randome movement
                    agent.SetDestination(RandomPos());
                    return;
            }
            

        }

        private Vector3 RandomPos()
        {
            Vector3 dir = Random.insideUnitSphere * 20;
            dir += transform.position;

            NavMeshHit navHit;

            NavMesh.SamplePosition(dir, out navHit, 20, -1);

            return navHit.position;
        }
    }
}
