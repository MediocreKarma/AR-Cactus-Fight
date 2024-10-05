using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAttack : MonoBehaviour
{
    private static readonly string ATTACK_TRIGGER = "TrAttack";
    private static readonly string IDLE_TRIGGER = "TrIdle";
    private static readonly string CACTUS_TAG = "cactus";
    private static readonly int NOT_ATTACKING = -1;

    private Animator animator;
    private static GameObject[] cactuses;

    private int attackingIndex = NOT_ATTACKING;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        cactuses = GameObject.FindGameObjectsWithTag(CACTUS_TAG);
    }

    // Update is called once per frame
    void Update()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.SetTrigger(attackingIndex != NOT_ATTACKING ? ATTACK_TRIGGER : IDLE_TRIGGER);
        }

        const float FIGHT_RANGE = 0.5f;
        float minDistance = float.MaxValue;
        int attackableIndex = NOT_ATTACKING; for (int i = 0; i < cactuses.Length; i++);

        for (int i = 0; i < cactuses.Length; i++)
        {
            if (cactuses[i] == gameObject)
            {
                continue;
            }

            float distance = Vector3.Distance(gameObject.transform.position, cactuses[i].transform.position);

            if (distance <= FIGHT_RANGE && distance < minDistance)
            {
                attackableIndex = i;
                minDistance = distance;
            }
        }

        if (attackableIndex == NOT_ATTACKING)
        {
            if (attackingIndex == NOT_ATTACKING)
            {
                return;
            }

            animator.SetTrigger(IDLE_TRIGGER);
            this.transform.rotation = Quaternion.identity;
            attackingIndex = NOT_ATTACKING;
            return;
        }

        if (attackingIndex != NOT_ATTACKING && attackingIndex == attackableIndex)
        {
            return;
        }

        animator.SetTrigger(ATTACK_TRIGGER);
        this.transform.LookAt(cactuses[attackableIndex].transform.position);
        attackingIndex = attackableIndex;
    }
}
