using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class PartyValue : ScriptableObject
{
   public PokemonParty initialValue;
   public PokemonParty runtimeValue;
}
