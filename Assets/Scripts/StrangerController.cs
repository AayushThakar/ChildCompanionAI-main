using System.Collections;
using UnityEngine;

public class StrangerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float approachSpeed = 1.5f;
    public float randomMovementRadius = 5f;
    public float stopDistance = 0.5f; // Distance in front of the child where the stranger will stop

    private GameObject Child;
    private bool approachingChild = false;

    public void Initialize(GameObject childObject, float radius)
    {
        Child = childObject;
        randomMovementRadius = radius;
        StartCoroutine(RandomMovementRoutine());
        StartCoroutine(ApproachChildRoutine());
    }

    IEnumerator RandomMovementRoutine()
    {
        while (true)
        {
            if (!approachingChild)
            {
                Debug.Log("Stranger is moving randomly.");
                Vector3 randomPosition = transform.position + new Vector3(
                    Random.Range(-randomMovementRadius, randomMovementRadius),
                    0,
                    Random.Range(-randomMovementRadius, randomMovementRadius)
                );
                yield return MoveToPosition(randomPosition, moveSpeed);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ApproachChildRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            approachingChild = true;
            Debug.Log("Stranger is approaching the child.");

            // Move towards the position in front of the child
            yield return MoveToPositionInFrontOfChild();

            // After approaching, return to random movement
            approachingChild = false;
        }
    }

    IEnumerator MoveToPositionInFrontOfChild()
    {
        while (Child != null)
        {
            // Calculate the direction from the stranger to the child
            Vector3 directionToChild = (Child.transform.position - transform.position).normalized;

            // Calculate the target position in front of the child, at the stop distance
            Vector3 targetPosition = Child.transform.position - directionToChild * stopDistance;

            // Move towards the target position
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position += directionToChild * approachSpeed * Time.deltaTime;
                yield return null;
            }
            else
            {
                // Stop when close enough to the target position
                break;
            }
        }
    }
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Child"))
    {
        Debug.Log("OnTriggerEnter: Stranger entered child's proximity.");
        if (DogController.Instance == null)
        {
            Debug.LogError("DogController.Instance is null!");
        }
        else
        {
            DogController.Instance.OnStrangerDetected(transform.position);
        }
    }
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Child"))
    {
        Debug.Log("OnTriggerExit: Stranger exited child's proximity.");
        DogController.Instance?.OnStrangerLeft();
    }
}
}