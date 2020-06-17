using UnityEngine;

public class RandomAnimationDelay : MonoBehaviour
{
    private void OnEnable()
    {
        var anim = GetComponent<Animator>();
        anim.SetFloat("Offset", Random.Range(0.0f, 1.0f));
    }
}
