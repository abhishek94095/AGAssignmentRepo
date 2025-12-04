using UnityEngine;
using DG.Tweening;
using System;

public class CardView : MonoBehaviour
{
    public int CardId { get; private set; }

    [SerializeField] private SpriteRenderer backSprite, cardSprite, cardObject; // SpriteRenderer on BackBG

    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private Ease flipEase = Ease.OutQuad;

    private bool isFrontShowing;
    private bool isAnimating;
    private Vector3 originalScale;

    // === NEW ===
    // who is allowed to flip this card (set from controller)
    public Func<CardView, bool> CanFlipCallback;

    // invoked when flip animation finishes
    public Action<CardView> OnFlipCompleted;

    public bool IsFrontShowing => isFrontShowing;   // read-only for controller

    private void Awake()
    {
        originalScale = transform.localScale;

        // Default state: face down
        ShowBack();
    }

    // Only sprite is passed. (Using as "front" image as you currently do.)
    public void Initialize(int id, Sprite cardSprite)
    {
        CardId = id;

        if (this.backSprite != null && cardSprite != null)
            this.cardSprite.sprite = cardSprite;
    }

    private void ShowFront()
    {
        // your current front/back logic
        cardObject.enabled = false;
        isFrontShowing = true;
    }

    private void ShowBack()
    {
        cardObject.enabled = true;
        isFrontShowing = false;
    }

    public void Flip()
    {
        // if (isAnimating) return;
        // isAnimating = true;

        bool nextIsFront = !isFrontShowing;

        Sequence seq = DOTween.Sequence();

        // Scale to 0 (flip mid-way)
        seq.Append(transform.DOScaleX(0f, flipDuration * 0.5f).SetEase(flipEase));

        // Swap which side is active
        seq.AppendCallback(() =>
        {
            if (nextIsFront) ShowFront();
            else ShowBack();
        });

        // Scale back to original
        seq.Append(transform.DOScaleX(1f, flipDuration * 0.5f).SetEase(flipEase));

        seq.OnComplete(() =>
        {
            // isAnimating = false;

            // === NEW ===
            OnFlipCompleted?.Invoke(this);
        });
    }

    private void OnMouseDown()
    {
        // === UPDATED: go through controller if assigned ===
        if (CanFlipCallback != null && !CanFlipCallback(this))
            return;

        Flip();
    }
}
