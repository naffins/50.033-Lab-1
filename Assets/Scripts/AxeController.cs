using UnityEngine;

public class AxeController : ResettableStateControllerBase {

    private const float swingPeriod = 0.125F;
    private const float swingAmplitude = 75F;

    private GameObject child;
    private BoxCollider2D childBoxCollider2d;
    private bool isActive = false;
    private bool clockwiseSwingDirection = false;
    private float swingElapsedTime = 0F;
    private Vector3 currentRotation = new Vector3(0F,0F,0F);

    void Awake() {
        isActive = false;
        clockwiseSwingDirection = false;
        swingElapsedTime = 0F;
        
        RotateTo(new Vector3(0F,0F,0F));
    }

    protected override void OnInitializeStates()
    {
        foreach (Transform t in transform) {
            child = t.gameObject;
            childBoxCollider2d = child.GetComponent<BoxCollider2D>();
        }
        isActive = false;
        clockwiseSwingDirection = false;
        child.SetActive(false);
        childBoxCollider2d.enabled = false;
        swingElapsedTime = 0F;
        
        RotateTo(new Vector3(0F,0F,0F));

    }

    protected override void OnPause(){
        childBoxCollider2d.enabled = false;
    }

    protected override void OnResume(){
        childBoxCollider2d.enabled = true;
    }

    void FixedUpdate() {
        if (IsPaused()) return;
        if (!isActive) return;

        // Do swingy swingy shit here
        swingElapsedTime += Time.deltaTime;
        if (swingElapsedTime>=swingPeriod) {
            Deactivate();
            return;   
        }
        
        Vector3 targetRotation = new Vector3(0F,0F,swingAmplitude) * (0.5F - (swingElapsedTime/swingPeriod))
            * 2F * (clockwiseSwingDirection? 1F:-1F);
        RotateTo(targetRotation);

    }

    public void Activate(bool isClockwise) {
        if (IsPaused()) return;
        if (isActive) return;

        isActive = true;
        clockwiseSwingDirection = isClockwise;
        RotateTo(new Vector3(0F,0F,swingAmplitude) * (clockwiseSwingDirection? 1F: -1F));
        child.SetActive(true);
        swingElapsedTime = 0F;
    }

    private void Deactivate() {
        isActive = false;
        child.SetActive(false);
    }

    public void RotateTo(Vector3 targetRotation) {
        transform.Rotate(targetRotation - currentRotation);
        currentRotation = targetRotation;
    }

}