using UnityEngine;

public class riverSpirit : EnemyAI
{
    public Animator animator;
    public float animTimer;
    private float animTimerThreshhold = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animTimer += Time.deltaTime;
        if(animTimer < animTimerThreshhold) return;
        
        animTimer = 0;

        animator.SetBool("IsSearching", true);

    }
}
