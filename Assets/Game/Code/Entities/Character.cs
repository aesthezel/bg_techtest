using System;
using Game.Code.Entities;
using Game.Code.Shop;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Fields and Properties
    public Action<int> OnCreditsChanged;
    
    [SerializeField] 
    private CharacterSettings settings;

    [SerializeField] 
    private GameObject[] CharacterViewsReferences;

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
    #endregion
    
    #region Unity Workflow
    private void Start()
    {
        Credits = settings.StartingCredits;
        ConfigureViews();
    }
    #endregion

}
