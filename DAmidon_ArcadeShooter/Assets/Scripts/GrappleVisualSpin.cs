using UnityEngine;

//Makes grappling cooler... i think i dunno im running out of ideas
public class GrappleVisualSpin : MonoBehaviour
{
    [Header("Outer Ring")]
    [SerializeField] GraplingHook hook;
    [SerializeField] Transform outerRing;
    [SerializeField] float rotSpeed;

    [Header("Grapple Snap")]
    [SerializeField] float snapSize;
    [SerializeField] ParticleSystem hooked;
    [SerializeField] Vector2 hoverSize;
    [SerializeField] Vector2 hookedSize;
    Vector2 setSize;

    bool knownBool;

    void Update()
    {
        outerRing.Rotate(0, 0, rotSpeed * Time.deltaTime);

        outerRing.transform.localScale = Vector2.Lerp(outerRing.transform.localScale, setSize, snapSize * Time.deltaTime);

        if(hook.isGrappling)
            setSize = hookedSize;
        else setSize = hoverSize;

        Hooked(hook.isGrappling);
    }

    void Hooked(bool newBool)
    {
        if(newBool != knownBool)
        {
            knownBool = newBool;
            if(newBool == true)
            {
                hooked.Play();
            }
        }
    }
}
