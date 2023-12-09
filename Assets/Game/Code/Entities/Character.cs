using System;
using System.Collections.Generic;
using Game.Code.Entities;
using Game.Code.Shop;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Character : MonoBehaviour
{
    #region Fields and Properties
    public Action<int> OnCreditsChanged;
    public Action<ClothingShop> OnInteractWithShop;
    
    [SerializeField] 
    private CharacterSettings settings;

    [SerializeField] 
    private GameObject[] CharacterViewsReferences;
    
    [SerializeField]
    private SpriteLibrary spriteLibrary;
    [SerializeField] 
    private CharacterPartsReference[] partsReferences;
    

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
    
    public void ObtainItems(int cost, ClothItem[] items)
    {
        if (cost < 0) cost = 0;
        Credits -= cost;
    }

    public void ChangeCloth(string category, string label)
    {
        var foundReference = Array.Find(partsReferences, part => part.Category == category);
        foundReference.SpriteResolver.SetCategoryAndLabel(category, label);
    }
    #endregion
    
    #region Unity Workflow
    private void Start()
    {
        Credits = settings.StartingCredits;
        
        if(settings.StartingItems.Length != 0)
            CurrentItems.AddRange(settings.StartingItems);
        
        ConfigureViews();
    }

    #endregion

}
