using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressController : MonoBehaviour {

    float height;
    [SerializeField] RectTransform IconR;
    [SerializeField] RectTransform IconL;
    void Start () {
        RectTransform rectTrans = GetComponent<RectTransform>();
        height = rectTrans.rect.height - IconR.rect.height;
        //test
        //StartCoroutine(test());
	}

    IEnumerator test()
    {
        float i = 0;
        while(i < 1.1)
        {
            yield return new WaitForSeconds(1f);
            SetRightValue(i);
            i += 0.1f;
        }
    }
	
    public void SetRightValue(float progress)
    {
        IconR.anchoredPosition = new Vector2(IconR.anchoredPosition.x, Mathf.Min(progress, 1.0f) * height);
    }

    public void SetLeftValue(float progress)
    {
        IconL.anchoredPosition = new Vector2(IconL.anchoredPosition.x, Mathf.Min(progress, 1.0f) * height);
    }



}
