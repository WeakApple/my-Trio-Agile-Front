using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Material OutlineMaterial;
    private Material OriginMaterial;
    private SpriteRenderer OriginRenderer;

    enum STATE
    {
        MOVE, IDLE
    }

    private STATE nowState= STATE.MOVE;


    private Vector2 targetPosition;
    private Vector2 startPosision;

    private static int MOVE_SPEED = 2;
    private float moveTime = 2.0f;
    private float elapsedTime = 0f;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        OriginRenderer = GetComponent<SpriteRenderer>();
        OriginMaterial = OriginRenderer.material;

        animator = GetComponent<Animator>();
    }

    void AnimByState()
    {
        switch (nowState)
        {
            case STATE.MOVE:
                animator.Play("Run");
                break;
            case STATE.IDLE:
                animator.Play("Idle");
                break;
            default:
                animator.Play("Idle");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimByState();

        if (nowState == STATE.MOVE)
        {
            if (startPosision != targetPosition)
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / moveTime);
                transform.position = Vector2.Lerp(startPosision, targetPosition, t);

                if (t >= 1f)
                {
                    nowState = STATE.IDLE;
                }

            } else
            {
                nowState = STATE.IDLE;
            }
        }
    }

    public void Test()
    {
        Debug.Log("hello world");
    }

    public void moveWorker(Vector2 _targetPosition)
    {
        nowState = STATE.MOVE;
        startPosision = transform.position;
        targetPosition = _targetPosition;

        moveTime = Vector2.Distance(_targetPosition, startPosision) / MOVE_SPEED;
        elapsedTime = 0;
    }

    void OnMouseDown()
    {
        EventManager.RunEvent(EventManager.EVENT_CODE.UNIT_SELECT, gameObject);
        //EventManager.RunUnitSelect(gameObject);
        OriginRenderer.material = OutlineMaterial;
    }
}
