using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public RectTransform player; // Player UI image
    public RectTransform[] waypoints; // All 38 waypoints as UI elements
    public int currentWaypointIndex = 0;
    public int remainingMoves = 10;
    public float moveSpeed = 1000f;

    public void MovePlayer(int steps)
    {
        if (remainingMoves <= 0 || isMoving) return;

        remainingMoves--;
        StartCoroutine(MoveOverSteps(steps));
    }

    private bool isMoving = false;

    IEnumerator MoveOverSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            Vector2 targetPos = waypoints[currentWaypointIndex].anchoredPosition;

            // Smooth movement toward the waypoint
            while (Vector2.Distance(player.anchoredPosition, targetPos) > 1f)
            {
                player.anchoredPosition = Vector2.MoveTowards(
                    player.anchoredPosition,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        isMoving = false;
    }

    private void Start()
    {
        if (waypoints.Length > 0 && player != null)
        {
            currentWaypointIndex = 0;
            player.anchoredPosition = waypoints[0].anchoredPosition;
        }
    }

}
