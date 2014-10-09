﻿using UnityEngine;
using System.Collections;

public class ClimbInput : MonoBehaviour {
	public bool isClimbing = false;
	public ClimbInput partner;
	public GameObject leftHand;
	public GameObject rightHand;
	private bool movingLeftHand = true;
	public float bodyBelowHands;
	public float maxArmDistance;
	public Vector3 handPos;
	public Color normalHandColor;
	public Color highlightHandColor;
	public float maxPartnerDistance;
	public bool moveUp;

	//Falling
	public Vector3 leftHang;
	public Vector3 rightHang;
	public bool lHandHanging = false;
	public bool rHandHanging = false;
	private float fallRate = 4.0f;

	void Awake()
	{
		MoveBodyBetweenHands();
		ResetHandColors();

		leftHang = new Vector3(-0.75f,-0.7f,0);

		rightHang = new Vector3(0.75f,-0.7f,0);

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			isClimbing = !isClimbing;
			ResetHandColors();
			
			/*if (partner != null)
			{
				partner.isClimbing = true;
			}*/
		}

		if (isClimbing)
		{
			moveUp = true;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				moveUp = false;
			}

			Handhold nextHandhold = null;
			handPos = leftHand.transform.position;
			if (!movingLeftHand)
			{
				handPos = rightHand.transform.position;
			}

			if (Input.GetButtonDown("Top"))
			{
				nextHandhold = HandholdManager.Instance.NearestHandhold(Handhold.ButtonType.Top, handPos, transform.position.y, moveUp);
			}
			else if (Input.GetButtonDown("Left"))
			{
				nextHandhold = HandholdManager.Instance.NearestHandhold(Handhold.ButtonType.Left, handPos, transform.position.y, moveUp);
			}
			else if (Input.GetButtonDown("Bottom"))
			{
				nextHandhold = HandholdManager.Instance.NearestHandhold(Handhold.ButtonType.Bottom, handPos, transform.position.y, moveUp);
			}
			else if (Input.GetButtonDown("Right"))
			{
				nextHandhold = HandholdManager.Instance.NearestHandhold(Handhold.ButtonType.Right, handPos, transform.position.y, moveUp);
			}

			if (nextHandhold != null && (nextHandhold.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(maxArmDistance, 2))
			{
				if (movingLeftHand)
				{
					leftHand.transform.position = nextHandhold.transform.position;
					leftHand.renderer.material.color = normalHandColor;
					rightHand.renderer.material.color = highlightHandColor;
					lHandHanging = false;
				}
				else
				{
					rightHand.transform.position = nextHandhold.transform.position;
					rightHand.renderer.material.color = normalHandColor;
					leftHand.renderer.material.color = highlightHandColor;
					rHandHanging = false;
				}

				movingLeftHand = !movingLeftHand;
				MoveBodyBetweenHands();
			}
		}
		else
		{
			if (leftHand.renderer.material.color != normalHandColor || rightHand.renderer.material.color != normalHandColor)
			{
				
			}
		}

		if(lHandHanging && rHandHanging)
		{
			transform.Translate(Vector3.down * Time.deltaTime * fallRate);
		}
	}// End of Update

	private void ResetHandColors()
	{
		leftHand.renderer.material.color = normalHandColor;
		rightHand.renderer.material.color = normalHandColor;
		if (isClimbing)
		{
			if (movingLeftHand)
			{
				leftHand.renderer.material.color = highlightHandColor;
			}
			else
			{
				rightHand.renderer.material.color = normalHandColor;
			}
		}
	}

	private void MoveBodyBetweenHands()
	{
		Vector3 newPosition = (leftHand.transform.position + rightHand.transform.position) / 2;
		newPosition.y -= bodyBelowHands;
		if (partner != null && (newPosition - partner.transform.position).sqrMagnitude > Mathf.Pow(maxPartnerDistance, 2))
		{
			Vector3 fromPartner = (newPosition - partner.transform.position).normalized;
			newPosition = partner.transform.position + (fromPartner * maxPartnerDistance);
		}
		leftHand.transform.position -= (newPosition - transform.position);
		rightHand.transform.position -= (newPosition - transform.position);
		transform.position = newPosition;
	}

	public void SlipLeft()
	{
		leftHand.transform.localPosition = leftHang;
		lHandHanging = true;
	}

	public void SlipRight()
	{
		rightHand.transform.localPosition = rightHang;
		rHandHanging = true;
	}
}
