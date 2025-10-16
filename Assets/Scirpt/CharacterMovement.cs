using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    private Queue<Vector3> path = new Queue<Vector3>();
    private bool isMoving = false;


    private void Update()
    {
        if (!isMoving ) return;

        if ( path.Count > 0 )
        {
            Vector3 targetPos = path.Peek();
            transform.position = Vector3.MoveTowards(transform.position,targetPos,moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position,targetPos) < 0.01f )
            {
                transform.position = targetPos;
                path.Dequeue();
            }

        }
        else
        {
            isMoving = false;
        }
        
    }
    public void MoveToDestination(Vector3 targetPos)
    {
        path.Clear();


        Vector3 startPos = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
        Vector3 endPos = new Vector3(Mathf.RoundToInt(targetPos.x), transform.position.y, Mathf.RoundToInt(targetPos.z));

        int currentX = Mathf.RoundToInt(startPos.x);
        int currentZ = Mathf.RoundToInt(startPos.z);
        int targetX = Mathf.RoundToInt(endPos.x);
        int targetZ = Mathf.RoundToInt(endPos.z);

        while (currentX != targetX || currentZ != targetZ)
        {
            if (currentX != targetX)
            {
                currentX += (int)Mathf.Sign(targetX - currentX);
            }
            if (currentZ != targetZ)
            {
                currentZ += (int)Mathf.Sign(targetZ - currentZ);
            }
            path.Enqueue(new Vector3(currentX, startPos.y, currentZ));
        }
        if (path.Count > 0)
        {
            isMoving = true;
        }
    }

}
