using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopUp : MonoBehaviour
{
    public Transform imagesParent;
    public Transform textsParent;
    public GameObject imageColumnPrefab;
    public GameObject textColumnPrefab;

    public static InfoPopUp instance;

    Vector2 originalSize;
    Vector2 originalPosition;
    float targetSize;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Close();
        }
    }

    public void Close()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Display a dynamic info box with given images and text
    /// </summary>
    /// <param name="images">Icon for info column</param>
    /// <param name="info">Text displayed per column</param>
    public void Open(List<Sprite> images, List<string> info, Transform targetTransform)
    {
        transform.position = targetTransform.position;
        StopAllCoroutines(); //THIS MIGHT BE A BAD IDEA!!
        GetComponent<Animator>().enabled = true;

        if (originalSize != Vector2.zero)
        {
            transform.GetComponent<RectTransform>().sizeDelta = originalSize;
        }

        gameObject.SetActive(false);

        gameObject.SetActive(true);

        foreach (Transform t in textsParent)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in imagesParent)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < info.Count; i++)
        {
            if (images.Count > i)
            {
                Image img = Instantiate(imageColumnPrefab, imagesParent).GetComponent<Image>();
                img.sprite = images[i];
            }
            Text txt = Instantiate(textColumnPrefab, textsParent).GetComponent<Text>();
            txt.text = info[i];
        }

        targetSize = info.Count * (textColumnPrefab.GetComponent<RectTransform>().sizeDelta.y + GetComponentInChildren<VerticalLayoutGroup>().spacing);
        CancelInvoke();
        Invoke("PopIn", 0.5f);
    }

    void PopIn()
    {
        if (!isActiveAndEnabled) return;
        GetComponent<Animator>().enabled = false;
        StartCoroutine(FadeIn());
    }


    IEnumerator FadeIn()
    {
        originalSize = GetComponent<RectTransform>().sizeDelta;
        originalPosition = transform.position;
        float t = 0;
        Vector2 target = originalSize;
        target.y += targetSize;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, originalPosition + Vector2.down * targetSize / 2, Time.deltaTime * target.y * 2);
            GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(GetComponent<RectTransform>().sizeDelta, target, Time.deltaTime * target.y * 4);
            yield return null;
        }
        transform.position = originalPosition + Vector2.down * targetSize / 2;
        GetComponent<RectTransform>().sizeDelta = target;

        yield return null;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            foreach (Text text in GetComponentsInChildren<Text>())
            {
                text.color = Color.Lerp(text.color, Color.clear, Time.deltaTime * 2);
            }
            foreach (Image img in GetComponentsInChildren<Image>())
            {
                img.color = Color.Lerp(img.color, Color.clear, Time.deltaTime * 2);
            }
            yield return null;
        }

        yield return null;
    }

}
