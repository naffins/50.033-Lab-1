using UnityEngine;

public class AxeController : ResettableStateControllerBase {

    // Time taken for axe to complete motion
    private const float swingPeriod = 0.125F;

    // Angle swung by axe on both sides of the y-axis
    private const float swingAmplitude = 75F;

    private GameObject child;
    private BoxCollider2D childBoxCollider2d;
    private bool isActive = false;
    // Rotation direction for current axe swing
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

        // Get the actual child axe
        foreach (Transform t in transform) {
            child = t.gameObject;
            childBoxCollider2d = child.GetComponent<BoxCollider2D>();
        }
        isActive = false;
        clockwiseSwingDirection = false;
        child.SetActive(false);
        childBoxCollider2d.enabled = false;

        // Reset the elapsed activation duration
        swingElapsedTime = 0F;
        

        // Set axe upright
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

        // Calculate how much time has passed since axe was activated
        swingElapsedTime += Time.deltaTime;
        if (swingElapsedTime>=swingPeriod) {
            Deactivate();
            return;   
        }
        
        // Rotate the axe
        Vector3 targetRotation = new Vector3(0F,0F,swingAmplitude) * (0.5F - (swingElapsedTime/swingPeriod))
            * 2F * (clockwiseSwingDirection? 1F:-1F);
        RotateTo(targetRotation);

    }

    // Activate the axe; initiate the swing
    public void Activate(bool isClockwise) {
        if (IsPaused()) return;
        if (isActive) return;

        isActive = true;
        clockwiseSwingDirection = isClockwise;

        // Set axe to be at starting position
        RotateTo(new Vector3(0F,0F,swingAmplitude) * (clockwiseSwingDirection? 1F: -1F));

        // Activate the actual axe with bounding box
        child.SetActive(true);

        // Reset stopwatch tracking time since axe was activated
        swingElapsedTime = 0F;
    }

    // Deactivate the axe
    private void Deactivate() {
        isActive = false;
        child.SetActive(false);
    }

    // Perform rotation to a target rotation wrt world coordinates
    public void RotateTo(Vector3 targetRotation) {
        transform.Rotate(targetRotation - currentRotation);
        currentRotation = targetRotation;
    }

}