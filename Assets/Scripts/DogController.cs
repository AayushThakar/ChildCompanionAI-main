using UnityEngine;
using TMPro;
using System.Collections;

public class DogController : MonoBehaviour
{
    public static DogController Instance { get; private set; }

    public float followSpeed = 2f;
    public Vector3 offset = new Vector3(-1f, 0, -1f);
    public AudioSource barkAudio; 
    public float nudgeForce = 0.2f; // Force to apply when nudging
    private bool isNudging = false;
    public float nudgeDuration = 0.2f;
    public TextMeshProUGUI strangerAlertText;
    private Animator textAnimator;

    private GameObject Child;
    private int strangersNearby = 0;

    void Awake()
    {
        Instance = this;
        Debug.Log("DogController Instance set.");
    }

    void Start()
    {
        // Find the TextMeshProUGUI object by its name
        strangerAlertText = GameObject.Find("StrangerText").GetComponent<TextMeshProUGUI>();

        // Get the Animator component
        if (strangerAlertText != null)
        {
            textAnimator = strangerAlertText.GetComponent<Animator>();
        }
    }

    public void Initialize(GameObject childObject)
    {
        Child = childObject;
    }

    void Update()
    {
        FollowChildWithOffset();
    }

    void FollowChildWithOffset()
    {
        if (Child != null)
        {
            Vector3 targetPosition = Child.transform.position + offset;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }

    public void OnStrangerDetected(Vector3 strangerPosition)

    {
        

        strangersNearby++;
        Debug.Log("DogController: Stranger detected. Strangers nearby: " + strangersNearby);

        if (barkAudio != null && !barkAudio.isPlaying)
        {
            barkAudio.Play();
        }
        // Nudge the child away from the stranger
        if (Child != null && !isNudging)
        {
            isNudging = true;
            StartCoroutine(NudgeChild(strangerPosition)); // Start the coroutine ONLY once
            Vector3 direction = (Child.transform.position - strangerPosition).normalized;
            Child.GetComponent<Rigidbody>().AddForce(direction * nudgeForce, ForceMode.Impulse);
        }

        // Show the text with animation
        if (strangerAlertText != null && textAnimator != null) // Check if animator is assigned
        {
            strangerAlertText.text = "This is a stranger!";
            strangerAlertText.gameObject.SetActive(true);
            textAnimator.Play("FadeIn"); 
        }
    }

    public void OnStrangerLeft()
    {
        strangersNearby--;
        if (strangersNearby <= 0)
        {
            strangersNearby = 0;
            Debug.Log("DogController: No more strangers nearby.");
            if (barkAudio != null && barkAudio.isPlaying)
            {
                barkAudio.Stop();
            }

            // Hide the text with animation
            if (strangerAlertText != null && textAnimator != null) // Check if animator is assigned
            {
                textAnimator.Play("FadeOut");
                StartCoroutine(HideTextAfterDelay(1f)); 
            }
        }
        else
        {
            Debug.Log("DogController: Stranger left. Strangers nearby: " + strangersNearby);
        }
    }

    IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        strangerAlertText.gameObject.SetActive(false);
    }
     IEnumerator NudgeChild(Vector3 strangerPosition)
    {
        float elapsedTime = 0f;


        while (elapsedTime < nudgeDuration && Child != null) // Added child null check
        {
            Vector3 direction = (Child.transform.position - strangerPosition).normalized;
            Child.GetComponent<Rigidbody>().AddForce(direction * nudgeForce * Time.deltaTime, ForceMode.Force);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isNudging = false; // Reset the flag after nudging is complete
    }
}