using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections; // Required for IEnumerator

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private bool isGrounded;
    private float elapsedTime = 0f;
    private bool isRunning = true; // Timer starts running
    private bool isBoosted = false;

    public float speed = 0;
    public float jumpForce = 5f;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI timerText; // Timer UI
    public GameObject winTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        isGrounded = true;
        SetCountText();
        winTextObject.SetActive(false);
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
        else if (other.gameObject.CompareTag("SpeedBoost"))
        {
            other.gameObject.SetActive(false);
            if (!isBoosted) StartCoroutine(SpeedBoost());
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 11)
        {
            winTextObject.SetActive(true);
            isRunning = false; // Stop the timer
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            isRunning = false; // Stop the timer
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    IEnumerator SpeedBoost()
    {
        isBoosted = true;
        float originalSpeed = speed;
        speed *= 2; // Double the speed
        yield return new WaitForSeconds(5); // Boost lasts for 5 seconds
        speed = originalSpeed; // Reset to normal speed
        isBoosted = false;
    }
}
