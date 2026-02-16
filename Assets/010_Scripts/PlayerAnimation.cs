using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private PlayerControllerForRigidBody m_playerController;
    [SerializeField] private PlayerRaise m_playerRaise;
    [SerializeField] private PlayerAttackAction m_playerAttackAction;
    [SerializeField] private PlayerInteractDetection m_playerInteractDetection;
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private SpriteRenderer m_handRenderer;

    float m_oldInput = 0.0f;

    private void Reset()
    {
        m_playerController = this.transform.parent.GetComponent<PlayerControllerForRigidBody>();
        m_playerRaise = m_playerController.GetComponentInChildren<PlayerRaise>();
        m_playerAttackAction = m_playerController.GetComponentInChildren<PlayerAttackAction>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    void Update()
    {
        m_animator.SetFloat("move", Mathf.Abs(m_playerController.DirectionLocal.x));

        if (m_playerController.DirectionLocal.x == 1)
        {
            m_spriteRenderer.flipX = false;
            m_handRenderer.flipX = false;
        }
        else if (m_playerController.DirectionLocal.x == -1)
        {
            m_spriteRenderer.flipX = true;
            m_handRenderer.flipX = true;
        }

        m_oldInput = m_playerController.DirectionLocal.x;

        // Animator‚Ì•û‚Åue-5v‚È‚Ç‚Ì‹É¬”‚Í‚P‚æ‚è‘å‚«‚¢‚ÉŒë”F‚·‚é‚Ì‚Å¬”“_‚QŒ…ˆÈ‰º‚ÍØ‚Á‚Ä‚¨‚­
        float airVelocity = ((float)Math.Round(m_playerController.Velocity.y, 2));

        m_animator.SetFloat("AirVelocity", airVelocity);
        m_animator.SetBool("Raise", m_playerRaise.isRaise);
        m_animator.SetBool("IsStan", m_playerController.IsStan);
        m_animator.SetBool("Attack", m_playerAttackAction.isAttacking);
        m_animator.SetBool("Cleaning", m_playerInteractDetection.isInteract);
        m_animator.SetBool("IsGround", m_playerController.IsGround);
        
        // ˜r‚ğ•\¦
        m_handRenderer.gameObject.SetActive(m_playerRaise.isRaise);
    }
}
