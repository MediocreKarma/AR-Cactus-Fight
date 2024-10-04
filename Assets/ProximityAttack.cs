using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAttack : MonoBehaviour
{
    private static readonly string ATTACK_TRIGGER = "TrAttack";
    private static readonly string IDLE_TRIGGER = "TrIdle";
    private static readonly string CACTUS_TAG = "cactus";

    private Animator animator;
    private static GameObject[] cactuses;

    private float[] distances;
    private bool isAttacking = false;
    private int k = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        cactuses = GameObject.FindGameObjectsWithTag(CACTUS_TAG);
        distances = new float[cactuses.Length];
    }

    // Update is called once per frame
    void Update()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.SetTrigger(isAttacking ? ATTACK_TRIGGER : IDLE_TRIGGER);

            Debug.Log("Resetting trigger");
        }

        for (int i = 0; i < cactuses.Length; i++)
        {
            distances[i] = Vector3.Distance(gameObject.transform.position, cactuses[i].transform.position);
        }

        const float FIGHT_RANGE = 0.5f;

        int attackableIndex = -1;
        for (int i = 0; i < cactuses.Length; i++)
        {
            if (cactuses[i] == gameObject)
            {
                continue;
            }

            if (distances[i] <= FIGHT_RANGE)
            {
                attackableIndex = i;
                break;
            }
        }

        if (attackableIndex == -1)
        {
            if (!isAttacking)
            {
                return;
            }

            animator.SetTrigger(IDLE_TRIGGER);
            this.transform.rotation = Quaternion.identity;
            isAttacking = false;
            return;
        }

        if (isAttacking)
        {
            return;
        }
        Debug.Log("Attacking " + attackableIndex.ToString());
        animator.SetTrigger(ATTACK_TRIGGER);
        this.transform.LookAt(cactuses[attackableIndex].transform.position);
        isAttacking = true;

    }
}
