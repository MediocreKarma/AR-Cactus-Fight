using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAI : MonoBehaviour
{
    private static readonly string ATTACK_TRIGGER = "TrAttack";
    private static readonly string IDLE_TRIGGER = "TrIdle";
    private static readonly string DEAD_TRIGGER = "TrDead";
    // private static readonly string CACTUS_TAG = "cactus";
    private static readonly int NOT_ATTACKING = -1;
    private static readonly int CACTUS_TOTAL_HP = 5;
    private static readonly float CACTUS_DEAD_TIMER = 10;
    private static readonly int CACTUS_ATTACK_DAMAGE = 1;

    private Transform cameraTransform;
    private Animator animator;
    private static readonly List<CactusAI> cactuses = new();

    private int attackingIndex = NOT_ATTACKING;
    private int hp = CACTUS_TOTAL_HP;
    private float timer;

    private void Awake()
    {
        cactuses.Add(this);
    }

    private void OnDestroy()
    {
        cactuses.Remove(this);
    }

    public void TakeDamage(int dmg)
    {
        if (!IsAlive())
        {
            return;
        }
        Debug.Log(this + " took damage, hp left: " + (hp - dmg));
        hp -= dmg;
        if (!IsAlive())
        {
            animator.SetTrigger(DEAD_TRIGGER);
            timer = 0;
            attackingIndex = NOT_ATTACKING;
        }
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!IsAlive())
        {
            if (timer < CACTUS_DEAD_TIMER)
            {
                return;
            }
            hp = CACTUS_TOTAL_HP;
            animator.SetTrigger(IDLE_TRIGGER); 
            attackingIndex = NOT_ATTACKING;   
            return; 
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            if (attackingIndex == NOT_ATTACKING)
            {
                animator.SetTrigger(IDLE_TRIGGER);
            }
            else
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                cactuses[attackingIndex].TakeDamage(CACTUS_ATTACK_DAMAGE);
            }
        }

        const float FIGHT_RANGE = 0.5f;
        float minDistance = float.MaxValue;
        int attackableIndex = NOT_ATTACKING;

        for (int i = 0; i < cactuses.Count; i++)
        {
            if (cactuses[i] == this || !cactuses[i].IsAlive())
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
            transform.LookAt(cameraTransform, Vector3.up);
            attackingIndex = NOT_ATTACKING;
            return;
        }

        if (attackingIndex == attackableIndex)
        {
            transform.LookAt(cactuses[attackableIndex].transform.position);
            return;
        }
        Debug.Log(this + " is attacking " + cactuses[attackableIndex]);
        animator.SetTrigger(ATTACK_TRIGGER);
        transform.LookAt(cactuses[attackableIndex].transform.position);
        attackingIndex = attackableIndex;
    }
}
