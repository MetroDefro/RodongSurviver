using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BackGroundMover : MonoBehaviour
{
    [SerializeField] private BoxCollider2D gameArea;
    [SerializeField] private Player player;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != gameArea)
            return;

        Vector3 playerPos = player.transform.position;
        Vector3 backgroundPos = transform.position;

        float distanceX = Mathf.Abs(playerPos.x - backgroundPos.x);
        float distanceY = Mathf.Abs(playerPos.y - backgroundPos.y);

        float directionX = playerPos.x - backgroundPos.x < 0 ? -1 : 1;
        float directionY = playerPos.y - backgroundPos.y < 0 ? -1 : 1;

        if (distanceX > distanceY)
            transform.Translate(Vector2.right * directionX * 40);
        else if (distanceX < distanceY)
            transform.Translate(Vector2.up * directionY * 40);
    }
}