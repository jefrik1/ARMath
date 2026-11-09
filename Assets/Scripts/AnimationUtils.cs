using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Util animasi sederhana tanpa dependency eksternal.
/// Fade, scale, move, shake, pulse — semua coroutine-based.
/// </summary>
public static class AnimationUtils
{
    /// <summary>Fade in GameObject (alpha 0 -> 1).</summary>
    public static IEnumerator FadeIn(GameObject target, float duration = 0.5f, bool useCanvasGroup = true)
    {
        CanvasGroup cg = useCanvasGroup ? target.GetComponent<CanvasGroup>() : null;
        if (cg == null && useCanvasGroup)
            cg = target.AddComponent<CanvasGroup>();

        float startAlpha = useCanvasGroup && cg != null ? cg.alpha : 1f;

        if (cg != null)
        {
            cg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            cg.alpha = 1f;
        }
    }

    /// <summary>Fade out GameObject (alpha 1 -> 0).</summary>
    public static IEnumerator FadeOut(GameObject target, float duration = 0.5f, bool useCanvasGroup = true)
    {
        CanvasGroup cg = useCanvasGroup ? target.GetComponent<CanvasGroup>() : null;
        if (cg == null && useCanvasGroup)
            cg = target.AddComponent<CanvasGroup>();

        if (cg != null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = 1f - Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            cg.alpha = 0f;
        }
    }

    /// <summary>Scale bounce (pop effect).</summary>
    public static IEnumerator ScaleBounce(GameObject target, float duration = 0.3f, float scaleFactor = 1.2f)
    {
        Vector3 originalScale = target.transform.localScale;
        float elapsed = 0f;

        // Naik
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (duration * 0.5f));
            target.transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleFactor, t);
            yield return null;
        }

        // Turun
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / (duration * 0.5f));
            target.transform.localScale = Vector3.Lerp(originalScale * scaleFactor, originalScale, t);
            yield return null;
        }

        target.transform.localScale = originalScale;
    }

    /// <summary>Shake effect (untuk popup error / salah).</summary>
    public static IEnumerator Shake(GameObject target, float intensity = 5f, float duration = 0.3f)
    {
        Vector3 originalPos = target.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float decay = 1f - (elapsed / duration);
            float x = Random.Range(-1f, 1f) * intensity * decay;
            float y = Random.Range(-1f, 1f) * intensity * decay;
            target.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }

        target.transform.localPosition = originalPos;
    }

    /// <summary>Pulse (besar kecil berulang).</summary>
    public static IEnumerator Pulse(GameObject target, float duration = 1f, float scaleMin = 0.9f, float scaleMax = 1.1f, int cycles = 2)
    {
        Vector3 originalScale = target.transform.localScale;
        for (int i = 0; i < cycles; i++)
        {
            float elapsed = 0f;
            while (elapsed < duration / cycles / 2f)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.PingPong(elapsed / (duration / cycles / 2f), 1f);
                target.transform.localScale = Vector3.Lerp(originalScale * scaleMin, originalScale * scaleMax, t);
                yield return null;
            }
        }
        target.transform.localScale = originalScale;
    }

    /// <summary>Move dari posisi A ke B (lerp).</summary>
    public static IEnumerator MoveTo(GameObject target, Vector3 destination, float duration = 0.5f, bool worldSpace = false)
    {
        Vector3 startPos = worldSpace ? target.transform.position : target.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (worldSpace)
                target.transform.position = Vector3.Lerp(startPos, destination, t);
            else
                target.transform.localPosition = Vector3.Lerp(startPos, destination, t);
            yield return null;
        }

        if (worldSpace)
            target.transform.position = destination;
        else
            target.transform.localPosition = destination;
    }

    /// <summary>Rotate objek (searah jarum jam).</summary>
    public static IEnumerator Rotate(GameObject target, Vector3 axis, float degrees, float duration = 1f)
    {
        Vector3 startRot = target.transform.localEulerAngles;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.transform.localEulerAngles = startRot + axis * degrees * t;
            yield return null;
        }

        target.transform.localEulerAngles = startRot + axis * degrees;
    }

    /// <summary>Flip sprite/material (mirror horizontal).</summary>
    public static void FlipHorizontal(GameObject target, bool flip)
    {
        Vector3 scale = target.transform.localScale;
        scale.x = flip ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        target.transform.localScale = scale;
    }

    /// <summary>Tampilkan objek dengan animasi scale pop.</summary>
    public static IEnumerator PopIn(GameObject target, float duration = 0.2f)
    {
        target.SetActive(true);
        target.transform.localScale = Vector3.zero;
        yield return ScaleBounce(target, duration, 1.3f);
    }

    /// <summary>Sembunyikan objek dengan fade out lalu destroy/disable.</summary>
    public static IEnumerator FadeOutAndDisable(GameObject target, float duration = 0.3f)
    {
        yield return FadeOut(target, duration);
        target.SetActive(false);
    }

    /// <summary>Highlight (brightness flash).</summary>
    public static IEnumerator Highlight(GameObject target, float duration = 0.2f, Color highlightColor = default)
    {
        Image img = target.GetComponent<Image>();
        if (img != null)
        {
            Color original = img.color;
            if (highlightColor == default)
                highlightColor = new Color(1f, 1f, 1f, 0.3f);
            img.color = highlightColor;
            yield return new WaitForSeconds(duration);
            img.color = original;
        }
    }

    /// <summary>Sequential: jalankan beberapa animasi berurutan.</summary>
    public static IEnumerator Sequence(params IEnumerator[] animations)
    {
        foreach (var anim in animations)
        {
            if (anim != null)
                yield return anim;
        }
    }

    /// <summary>Parallel: jalankan beberapa animasi bersamaan.</summary>
    public static IEnumerator Parallel(params IEnumerator[] animations)
    {
        float elapsed = 0f;
        bool allDone = false;

        // Jalankan semua
        Coroutine[] coroutines = new Coroutine[animations.Length];
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i] != null)
                coroutines[i] = MonoSingleton<MonoBehaviour>.Instance.StartCoroutine(animations[i]);
        }

        yield return new WaitForSeconds(10f); // max duration fallback

        allDone = true;
        // Cleanup
        for (int i = 0; i < coroutines.Length; i++)
        {
            if (coroutines[i] != null)
                MonoSingleton<MonoBehaviour>.Instance.StopCoroutine(coroutines[i]);
        }
    }
}

/// <summary>Singleton MonoBehaviour untuk memulai coroutine dari static class.</summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
}