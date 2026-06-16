using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using PlayerCore;
using SLS.ListUtilities;

[RequireComponent(typeof(Animator))]
public class ShelterCutsceneManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera lookingAtShelterCamera;
    [SerializeField] private Transform TheHand;
    [SerializeField] private Transform HandStretchyStart;
    [SerializeField] private Transform HandStretchyEnd;
    [SerializeField] private float handChaseSpeed = 15.0f;
    [SerializeField] private DictionaryS<string, AudioClip> SFX;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSourceSFX;
    [SerializeField] CinemachineImpulseSource impulse;
    [SerializeField] CinemachineImpulseDefinition slamImpulse;
    [SerializeField] CinemachineImpulseDefinition laughImpulse;
    [SerializeField] UltEvents.UltEvent activateEndless;

    public void StartCutscene()
    {
        animator.enabled = true;
        animator.Play("ShelterAnim_1");
        //if (isCutscenePlayed) return;
        //isCutscenePlayed = true;
        //
        //StartCoroutine(PlayCutsceneCoroutine());
    }

    /*
    private IEnumerator PlayCutsceneCoroutine()
    {
        // 1. Pause Player using the ActivityState property
        Player.ActivityState = Player.ActivityStates.Paused;

        // 2. Activate lookingAtShelterCamera to initiate transition
        if (lookingAtShelterCamera != null)
        {
            lookingAtShelterCamera.gameObject.SetActive(true);
            lookingAtShelterCamera.Priority = 20; // High priority to blend over standard cameras
        }

        // 3. Move ShadowHaze farther into background as camera pans
        if (shadowHaze != null)
        {
            float elapsed = 0f;
            Vector3 hazeStart = shadowHaze.transform.position;
            Vector3 hazeEnd = hazeStart + new Vector3(0, 0, shadowHazeMoveDistance);

            while (elapsed < cameraTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / cameraTransitionDuration);
                shadowHaze.transform.position = Vector3.Lerp(hazeStart, hazeEnd, t);
                yield return null;
            }
            shadowHaze.transform.position = hazeEnd;
        }
        else
        {
            yield return new WaitForSeconds(cameraTransitionDuration);
        }

        // 4. Play doors opening animation on THE SHELTER
        animator.SetTrigger("Open");

        // 5. Play frightening sound effect
        if (scarySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(scarySound);
        }

        // Wait a short moment for the doors to swing fully open and show the frightening visuals inside
        yield return new WaitForSeconds(1.5f);

        // 6. Spawn the giant black hand and begin the fast inescapable chase
        GameObject hand = null;
        if (giantBlackHandPrefab != null)
        {
            hand = Instantiate(giantBlackHandPrefab);
        }
        else
        {
            // Build the giant black hand procedurally if no prefab is provided
            hand = CreateProceduralHand();
        }

        // Position the hand in front of the shelter doors
        Vector3 handSpawnPos = transform.position + new Vector3(0, 0.5f, handStartOffsetZ);
        hand.transform.position = handSpawnPos;

        // Chase loop
        bool handReachedPlayer = false;
        while (!handReachedPlayer && Player.Exists)
        {
            Vector3 handPos = hand.transform.position;
            Vector3 playerPos = Player.Position;

            // Move fast along Z towards player
            handPos.z -= handChaseSpeed * Time.deltaTime;

            // Follow player horizontally (X-axis) and vertically (Y-axis)
            handPos.x = Mathf.MoveTowards(handPos.x, playerPos.x, handChaseSpeed * Time.deltaTime);
            handPos.y = Mathf.MoveTowards(handPos.y, playerPos.y + 0.5f, handChaseSpeed * Time.deltaTime);

            hand.transform.position = handPos;

            // Check if hand reached player
            if (handPos.z <= playerPos.z + 1.0f)
            {
                handReachedPlayer = true;
            }

            yield return null;
        }

        // 7. Transition quickly back to the normal camera once it reaches the player
        if (lookingAtShelterCamera != null)
        {
            lookingAtShelterCamera.Priority = 0;
            lookingAtShelterCamera.gameObject.SetActive(false);
        }
        Cameras.SetTargetVirtualCamera(Cameras.NormalCamera);

        // 8. Clean up hand
        if (hand != null)
        {
            Destroy(hand);
        }

        // 9. Restore player control
        Player.ActivityState = Player.ActivityStates.Active;
    }
    */

    private GameObject CreateProceduralHand()
    {
        GameObject hand = new GameObject("GiantBlackHand");

        // Find or create a black material
        Material blackMat = null;
#if UNITY_EDITOR
        blackMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Environment/Friend/ShadowHaze.mat");
        if (blackMat == null) blackMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Gaskellgames/GgCore/Runtime/Materials/GgPrototype_Black.mat");
#endif

        if (blackMat == null)
        {
            blackMat = new Material(Shader.Find("Standard"));
            blackMat.color = Color.black;
        }

        // Palm
        GameObject palm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        palm.name = "Palm";
        palm.transform.SetParent(hand.transform, false);
        palm.transform.localScale = new Vector3(2.5f, 0.6f, 2.5f);
        if (palm.GetComponent<Renderer>() != null) palm.GetComponent<Renderer>().sharedMaterial = blackMat;
        // Disable collider on children so they don't mess up physics
        if (palm.GetComponent<Collider>() != null) Destroy(palm.GetComponent<Collider>());

        // Fingers configuration: (localPos, localRotEuler, localScale, name)
        var fingers = new (Vector3 pos, Vector3 rot, Vector3 scale, string name)[]
        {
            (new Vector3(-1.1f, 0.2f, 0.2f), new Vector3(-30, -35, 0), new Vector3(0.5f, 1.2f, 0.5f), "Thumb"),
            (new Vector3(-0.6f, 0.3f, 1.3f), new Vector3(-45, 0, 0), new Vector3(0.45f, 1.5f, 0.45f), "Index"),
            (new Vector3(0.0f, 0.3f, 1.5f), new Vector3(-45, 0, 0), new Vector3(0.45f, 1.7f, 0.45f), "Middle"),
            (new Vector3(0.6f, 0.3f, 1.3f), new Vector3(-45, 0, 0), new Vector3(0.45f, 1.5f, 0.45f), "Ring"),
            (new Vector3(1.1f, 0.2f, 0.9f), new Vector3(-40, 15, 0), new Vector3(0.4f, 1.3f, 0.4f), "Pinky")
        };

        foreach (var f in fingers)
        {
            GameObject finger = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            finger.name = f.name;
            finger.transform.SetParent(hand.transform, false);
            finger.transform.localPosition = f.pos;
            finger.transform.localRotation = Quaternion.Euler(f.rot);
            finger.transform.localScale = f.scale;
            if (finger.GetComponent<Renderer>() != null) finger.GetComponent<Renderer>().sharedMaterial = blackMat;
            if (finger.GetComponent<Collider>() != null) Destroy(finger.GetComponent<Collider>());
        }

        return hand;
    }


    public void SetCutsceneCamera() => lookingAtShelterCamera.Priority = 20;
    public void SetNormalCamera() => lookingAtShelterCamera.Priority = -100;


    public void PausePlayer(string why)
    {
        Player.ActivityState = Player.ActivityStates.Paused;
    }
    public void PlayPlayer() => Player.ActivityState = Player.ActivityStates.Active;

    public void PlaySFX(string name) => audioSourceSFX.PlayOneShot(SFX[name]);

    public void DoShake(int i)
    {
        impulse.ImpulseDefinition = i switch
        {
            0 => slamImpulse,
            1 => laughImpulse
        };
        impulse.GenerateImpulse();
    }

    public void EndCutscene()
    {
        animator.Play("After");
        animator.enabled = false;
    }

    public void SendTHEHAND()
    {
        Enum().Begin(this);
        IEnumerator Enum()
        {
            float failsafe = 900;
            while (failsafe > 0)
            {
                failsafe -= Time.unscaledDeltaTime;
                TheHand.position += Vector3.back * handChaseSpeed * Time.unscaledDeltaTime;
                TheHand.position += Vector3.right * (Player.Position.x - TheHand.position.x);

                float d = Vector3.Distance(HandStretchyEnd.position, HandStretchyStart.position);
                HandStretchyStart.rotation = Quaternion.LookRotation(HandStretchyStart.position - HandStretchyEnd.position);
                HandStretchyStart.localScale = new(HandStretchyStart.localScale.x, HandStretchyStart.localScale.y, d * .5f);
                
                yield return null;
            }
        }
    }

    public void DecemberIsDead()
    {
        Overlay.OverALL.Color = Color.red;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}