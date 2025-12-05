using System;
using DG.Tweening;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public int CardId { get; private set; }
    public int CardIndex { get; private set; } = -1;
    public bool IsFrontShowing => isFrontSideShowing;

    [SerializeField] private SpriteRenderer FrontSpriteRenderer;
    [SerializeField] private SpriteRenderer CardBackRenderer;

    [SerializeField] private float FlipDuration = 0.3f;
    [SerializeField] private Ease FlipEase = Ease.OutQuad;

    public Func<CardView, bool> CanFlipCallback;
    public Action<CardView> FlipCompletedCallback;

    private bool isFrontSideShowing;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        ShowBack();
    }

    public void Initialize(int cardId, Sprite frontSprite)
    {
        CardId = cardId;

        if (FrontSpriteRenderer != null && frontSprite != null)
        {
            FrontSpriteRenderer.sprite = frontSprite;
        }
    }

    public void SetCardIndex(int index)
    {
        CardIndex = index;
    }

    public void Flip(bool isInitialFlip = false)
    {
        bool showFrontSide = !isFrontSideShowing;

        Sequence flipSequence = DOTween.Sequence();

        flipSequence.Append(transform.DOScaleX(0f, FlipDuration * 0.5f).SetEase(FlipEase));

        flipSequence.AppendCallback(() =>
        {
            if (showFrontSide)
            {
                ShowFront();
            }
            else
            {
                ShowBack();
            }
        });

        flipSequence.Append(transform.DOScaleX(originalScale.x, FlipDuration * 0.5f).SetEase(FlipEase));

        flipSequence.OnComplete(() =>
        {
            if (!isInitialFlip)
            {
                FlipCompletedCallback?.Invoke(this);
            }
        });
    }

    private void ShowFront()
    {
        if (CardBackRenderer != null)
        {
            CardBackRenderer.enabled = false;
        }

        isFrontSideShowing = true;
    }

    private void ShowBack()
    {
        if (CardBackRenderer != null)
        {
            CardBackRenderer.enabled = true;
        }

        isFrontSideShowing = false;
    }

    private void OnMouseDown()
    {
        if (CanFlipCallback != null && !CanFlipCallback(this))
        {
            return;
        }

        Flip();
        SoundManager.Instance?.Play(SoundType.Flip);
    }
}
