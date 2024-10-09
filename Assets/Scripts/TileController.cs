using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public bool Selected;
    private SpriteRenderer sprite;
    private Coroutine bounceCouroutine;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per fram

    public void ChangeColour(Color color)
    {
        sprite.color = color;
    }

    public void Select()
    {
        if (Selected == false)
        {
            Bounce();
            Selected = true;
        }

    }

    public void Deselect()
    {
        if (Selected == true)
        {
            Selected = false;
        }
    }

    public bool GetSelect()
    {
        return Selected;
    }

    public void Bounce()
    {
        if (bounceCouroutine != null)
        {
            StopCoroutine(bounceCouroutine); // Stop the previous bounce if it's still running
        }

        bounceCouroutine = StartCoroutine(BounceRoutine());
    }

    private IEnumerator BounceRoutine()
    {
        // Initial scale and target scale
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Scale up by 20%

        float duration = 0.2f; // Duration of the scale up and down
        float time = 0;

        // Scale up
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // Make sure it reaches the target scale
        transform.localScale = targetScale;

        // Reset time
        time = 0;

        // Scale down
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // Make sure it goes back to the original scale
        transform.localScale = originalScale;

        // End of coroutine
        bounceCouroutine = null;
    }

}
