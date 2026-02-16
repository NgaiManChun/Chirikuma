using UnityEngine;

[RequireComponent(typeof(PlayerControllerForRigidBody))]
public class FreeMover : MonoBehaviour
{
    private PlayerControllerForRigidBody m_playerController;

    private void Awake()
    {
        m_playerController = GetComponent<PlayerControllerForRigidBody>();
    }

    void Update()
    {
        m_playerController.Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Jump"))
            m_playerController.Jump();
    }
}
