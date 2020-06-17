using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private Animator animator;

    private void OnMouseEnter()
    {
        animator.SetBool("isOpen", true);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClientSend.MoveRequest(direction);
        }
    }

    private void OnMouseExit()
    {
        animator.SetBool("isOpen", false);
    }
}
