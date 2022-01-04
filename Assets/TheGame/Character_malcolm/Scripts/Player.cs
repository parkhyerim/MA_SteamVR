using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform targetDestination;
    [SerializeField] private Transform target;

    private NavMeshAgent agent;
    private Animator anim;
    private bool reachedDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    private void Start()
    {
        agent.SetDestination(targetDestination.position);

    }

    private void Update()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            float speed = 0;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                speed = 0;
                reachedDestination = true;
            }
            else
                speed = 1;

            anim.SetFloat("Speed", speed);
        }

        if (reachedDestination)
        {
            anim.SetTrigger("SitDown");
            agent.updateRotation = false;
            transform.rotation = targetDestination.rotation;
        }
          

    }

    private void OnAnimatorIK()
    {
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
    }
}
