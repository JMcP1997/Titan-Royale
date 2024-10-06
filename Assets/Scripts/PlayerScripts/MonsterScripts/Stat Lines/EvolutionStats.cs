using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New EvolutionStats", menuName = "PlayerData/PlayerEvolutionData")]
public class EvolutionStats : ScriptableObject
{
    //For purposes of ease, in this scriptable object, Evolution levels start at 0

    [Header("Character Controller Settings")]
    [SerializeField] Vector3 characterControllerCenter;
    [SerializeField] float characterControllerHeight;
    [SerializeField] float characterControllerRadius;

    public IEnumerator characterControllerLerp(CharacterController character)
    {
        float timeElapsed = 0;
        float lerpDuration = 3;
        while (timeElapsed < lerpDuration)
        {
            character.height = Mathf.Lerp(character.height, characterControllerHeight, timeElapsed / lerpDuration);
            character.radius = Mathf.Lerp(character.radius, characterControllerRadius, timeElapsed / lerpDuration);
            character.center = Vector3.Lerp(character.center, characterControllerCenter, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        character.height = characterControllerHeight;
    }
}
