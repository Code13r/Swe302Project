using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("cleared", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
