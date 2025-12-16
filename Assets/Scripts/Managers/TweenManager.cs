using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.littleShake = LittleShakeEffect;
    }
    public void LittleShakeEffect(Transform transform, float strenght, float duration, int vibrato, float randomness, bool fadeOut) //Item bool isEffectPlaying
    {
        //isEffectPlaying = true;
        GameManager.Instance.ItemEffectisPlaying = true;

        Vector3 originalScale = transform.localScale;

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakeScale(duration, new Vector3(0f, strenght, 0f), vibrato, randomness, fadeOut)); //.SetEase(Ease.OutBack);
        sequence.Append(transform.DOScale(originalScale, 0.2f).SetEase(Ease.Linear));
        sequence.AppendInterval(5f);
        sequence.OnComplete(() => {
            //isEffectPlaying = false;
            GameManager.Instance.ItemEffectisPlaying = false;    
            return;
        });
        
    }
}
