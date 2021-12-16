using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Vector3 m_GroundNormal;            // Whether or not the player is grounded.
	[SerializeField] private float m_GroundAngle;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private CapsuleCollider2D m_CapsuleCollider;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_CapsuleCollider = GetComponent<CapsuleCollider2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void Update()
	{
		m_CeilingCheck.localPosition = Vector3.up * (m_CapsuleCollider.offset.y + (m_CapsuleCollider.size.y / 2) -
		                                             (m_CapsuleCollider.size.x / 2));
		m_GroundCheck.localPosition = Vector3.up * (m_CapsuleCollider.offset.y - (m_CapsuleCollider.size.y / 2) +
		                                             (m_CapsuleCollider.size.x / 2));
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		RaycastHit2D hit = Physics2D.Raycast(transform.position, m_GroundCheck.localPosition, (m_CapsuleCollider.size.x / 2) + 0.05f, m_WhatIsGround);
		if (hit != null && hit.transform != null)
		{
			if (!hit.transform.IsChildOf(transform) && hit.transform != transform)
			{
				m_GroundNormal = hit.normal;
				m_GroundAngle = Vector2.Angle(hit.normal, Vector2.up);
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
			
		}
	}

	private Vector3 targetVelocity;


	public void Move(float move, float crouch, bool jump)
	{
		if (crouch != 0)
		{
			bool canCrouch = false;
			List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
			if (m_CapsuleCollider.GetContacts(contactPoints) <= 0)
			{
				canCrouch = true;
			}
			else
			{
				if (crouch > 0)
				{
					canCrouch = true;
					foreach (var contact in contactPoints)
					{
						if (contact.point.y > m_CeilingCheck.position.y)
						{
							canCrouch = false;
							break;
						}
					}
					
				}
				else
				{
					if (m_CapsuleCollider.size.y > m_CapsuleCollider.size.x)
					{
						canCrouch = true;
					}
				}
			}
			
			if (canCrouch)
			{
				Debug.Log("CAn crouch!");
				float previousSize = m_CapsuleCollider.size.y;
				m_CapsuleCollider.size = new Vector2(m_CapsuleCollider.size.x, Mathf.Clamp(m_CapsuleCollider.size.y + crouch, m_CapsuleCollider.size.x, Mathf.Infinity));
				m_CapsuleCollider.offset = new Vector2(m_CapsuleCollider.offset.x, (1-m_CapsuleCollider.size.y) * 0.5f);
				transform.position += Vector3.down * (previousSize - m_CapsuleCollider.size.y);
			}
			else
			{
					
				Debug.Log("CAnnot crouch!");
			}
		}
		// // If crouching, check to see if the character can stand up
		// if (!crouch)
		// {
		// 	// If the character has a ceiling preventing them from standing up, keep them crouching
		// 	if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
		// 	{
		// 		crouch = true;
		// 	}
		// }

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			// if (crouch)
			// {
			// 	if (!m_wasCrouching)
			// 	{
			// 		m_wasCrouching = true;
			// 		OnCrouchEvent.Invoke(true);
			// 	}
			//
			// 	// Reduce the speed by the crouchSpeed multiplier
			// 	move *= m_CrouchSpeed;
			//
			// 	// Disable one of the colliders when crouching
			// 	if (m_CrouchDisableCollider != null)
			// 		m_CrouchDisableCollider.enabled = false;
			// } else
			// {
			// 	// Enable the collider when not crouching
			// 	if (m_CrouchDisableCollider != null)
			// 		m_CrouchDisableCollider.enabled = true;
			//
			// 	if (m_wasCrouching)
			// 	{
			// 		m_wasCrouching = false;
			// 		OnCrouchEvent.Invoke(false);
			// 	}
			// }

			// Move the character by finding the target velocity
			targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			
			// the second argument, upwards, defaults to Vector3.up
			if (m_Grounded)
			{
				targetVelocity = Quaternion.Euler(0, 0, m_GroundAngle) * targetVelocity;
			}
			
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		
		m_Rigidbody2D.AddForce(Quaternion.Euler(0, 0, m_GroundAngle) * Physics.gravity);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(transform.position, transform.position + m_GroundNormal);
		Gizmos.DrawLine(transform.position, transform.position + targetVelocity);
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
