using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController instance;

    [Header("Animators")]
    public Animator MonsterAnimator;

    private void Awake()
    {
        instance = this;
    }

    public void HitReact()
    {
        bool isLeft = PlayerController.instance.IsPrimaryAttackAnimating();
        bool isRight = PlayerController.instance.IsSecondaryAttackAnimating();
        bool isGut = PlayerController.instance.IsHeavyAttackAnimating();

        StopCoroutine(HitReactPlay());
        if (isGut)
            StartCoroutine(HitReactPlay(0));
        if (isRight)
            StartCoroutine(HitReactPlay(1));
        if (isLeft)
            StartCoroutine(HitReactPlay(2));
    }

    IEnumerator HitReactPlay(int index = 0)
    {
        if(index == 0)
            MonsterAnimator.SetBool("HitGut", true);
        if (index == 1)
            MonsterAnimator.SetBool("HitRight", true);
        if (index == 2)
            MonsterAnimator.SetBool("HitLeft", true);

        yield return new WaitForSeconds(.5f);

        MonsterAnimator.SetBool("HitGut", false);        
        MonsterAnimator.SetBool("HitRight", false);        
        MonsterAnimator.SetBool("HitLeft", false);
    }
}
