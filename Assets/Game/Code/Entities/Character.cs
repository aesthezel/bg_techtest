using Game.Code.Entities;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] 
    private CharacterSettings settings;
    public int Credits { get; private set; }

    private void Start()
    {
        Credits = settings.StartingCredits;
    }
}
