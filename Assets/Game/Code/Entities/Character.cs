using System;
using System.Collections.Generic;
using System.Linq;
using Game.Code.Entities;
using Game.Code.Shop;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;

public class Character : MonoBehaviour
{
    #region Fields and Properties
    public Action<int> OnCreditsChanged;
    public Action<ClothingShop> OnInteractWithShop;
    public Action<List<ClothItem>> OnItemsUpdate;
    
    [SerializeField] 
    private CharacterSettings settings;

    [SerializeField] 
    private GameObject[] CharacterViewsReferences;
    
    [SerializeField]
    private SpriteLibrary spriteLibrary;
    [SerializeField] 
    private CharacterPartsReference[] partsReferences;
    
    [Header("Character Behaviours")] 
    [SerializeField]
    private CharacterMovement characterMovement;
    [SerializeField]
    private CharacterAnimation characterAnimation;
    [SerializeField] 
    private CharacterInteract characterInteract;
    

    private int _credits;
    public int Credits
    {
        get => _credits;
        private set
        {
            _credits = Mathf.Max(value, 0);
            OnCreditsChanged?.Invoke(_credits);
        }
    }

    public List<ClothItem> CurrentItems { get; private set; } = new();

    #endregion

    #region Methods
    private void ConfigureViews()
    {
        if (CharacterViewsReferences.Length == 0) return;
        
        foreach (var reference in CharacterViewsReferences)
        {
            var viewComponent = reference.GetComponent<ICharacterView>();
            viewComponent?.Configure(this);
        }
    }

    public void InteractMerchant(ClothingShop shop)
    {
        OnInteractWithShop?.Invoke(shop);
    }
    
    public void ObtainItems(int cost, ClothItem[] items)
    {
        if (cost < 0) cost = 0;
        Credits -= cost;
        
        CurrentItems.AddRange(items);
        OnItemsUpdate?.Invoke(CurrentItems);
    }

    public void TradeItems(int revenue, IEnumerable<ClothItem> items)
    {
        Credits += revenue;

        var itemsToRemove = CurrentItems.Intersect(items);
        CurrentItems.RemoveAll(itemsToRemove.Contains);
        OnItemsUpdate?.Invoke(CurrentItems);
    }

    public void ChangeCloth(string category, string label)
    {
        var foundReference = Array.Find(partsReferences, part => part.Category == category);
        foundReference.SpriteResolver.SetCategoryAndLabel(category, label);
    }

    public void TryOpenStore(GameObject possibleObject)
    {
        var shop = possibleObject.GetComponent<ClothingShop>();
        if (shop is null) return;

        if (shop.IsOpen) return;
        shop.Open(this);
        OnInteractWithShop?.Invoke(shop);
    }
    #endregion
    
    #region Unity Workflow
    private void Start()
    {
        Credits = settings.StartingCredits;
        
        if(settings.StartingItems.Length != 0)
            CurrentItems.AddRange(settings.StartingItems);
        
        ConfigureViews();

        characterInteract.Configure(this);
    }
    
    private void LateUpdate()
    {
        if (characterMovement.IsMoving)
        {
            characterAnimation.PerformWalk();
        }
        else
        {
            characterAnimation.PerformIdle();
        }
    }
    #endregion
}
