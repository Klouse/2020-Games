using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public void ChooseAnimation(string animationName, Animator animator, bool enable = true)
    {
        if (animator.parameterCount > 0)
        {
            try
            {
                // set the requested animation to the desired enabled state
                if (animator.GetBool(animationName) != enable)
                {
                    animator.SetBool(animationName, enable);
                    // turn off all the non requested animation bools
                    for (int anim = 0; anim < animator.parameterCount; anim++)
                    {
                        if (animator.GetBool(anim) && animator.GetParameter(anim).name != animationName)
                        {
                            animator.SetBool(anim, false);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
    public void ChooseAnimation(string animationName1, string animationName2, Animator animator, bool enable = true)
    {
        Debug.Log("Choosing animation");
        if (animator.parameterCount > 0)
        {
            Debug.Log("parameters found");
            try
            {
                bool flipped = false;
                // set the requested animation to the desired enabled state
                if (animator.GetBool(animationName1) != enable)
                {
                    Debug.Log($"Setting bool {animationName1}");
                    animator.SetBool(animationName1, enable);
                    Debug.Log("Bool set successful");
                    flipped = true;
                }

                // set the requested animation to the desired enabled state
                if (animator.GetBool(animationName2) != enable)
                {
                    Debug.Log($"Setting bool {animationName2}");
                    animator.SetBool(animationName2, enable);
                    Debug.Log("Bool set successful");
                    flipped = true;
                }
                if (flipped)
                {
                    Debug.Log("Bool was flipped, setting other animations to false");
                    // turn off all the non requested animation bools
                    for (int anim = 0; anim < animator.parameterCount; anim++)
                    {
                        Debug.Log($"Checking parameter: {animator.GetParameter(anim).name}");
                        if (animator.GetBool(anim) == enable && animator.GetParameter(anim).name != animationName1 && animator.GetParameter(anim).name != animationName2)
                        {
                            Debug.Log($"Animation setting to false");
                            animator.SetBool(anim, false);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
