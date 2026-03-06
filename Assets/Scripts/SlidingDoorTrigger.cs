using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SlidingDoorTrigger : MonoBehaviour
{
    [Header("Kapý Referansý")]
    public Transform doorTransform;

    [Header("Pozisyonlar (World Space)")]
    public Vector3 closedPosition;
    public Vector3 openPosition;

    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 2f;

    Coroutine moveRoutine;
    int insideCount = 0; // Ayný anda birden fazla enemy için güvenli

    private void Start()
    {
        if (doorTransform != null)
            doorTransform.position = closedPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        insideCount++;

        if (insideCount == 1)
            OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        insideCount--;

        if (insideCount <= 0)
        {
            insideCount = 0;
            CloseDoor();
        }
    }
    public void OpenDoor()
    {
        MoveDoor(openPosition);
    }

    public void CloseDoor()
    {
        MoveDoor(closedPosition);
    }
    void MoveDoor(Vector3 targetPos)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine(targetPos));
    }

    IEnumerator MoveRoutine(Vector3 target)
    {
        if (doorTransform == null)
            yield break;

        while ((doorTransform.position - target).sqrMagnitude > 0.0001f)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(bc.center, bc.size);
    }
#endif
}
