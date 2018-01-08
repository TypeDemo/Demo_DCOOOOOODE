using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState : StateMachineBehaviour {

    public string animName;
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.ResetTrigger(animName);
    }

}
