using System.Collections;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public string playerName;
    [HideInInspector] public int playerID;
    [SerializeField] private TextMeshPro nameTag;

    [Header("Spawn settings: ")]
    [SerializeField] private ParticleSystem respawnMiddle;
    [SerializeField] private ParticleSystem respawnCircle;
    [SerializeField] private ParticleSystem respawnGlow;
    [SerializeField] private ParticleSystem respawnDust;

    private float t = 0.0f;

    public void Initialize(Color c, int id, string playerName)
    {
        playerID = id;
        SetName(playerName);
        SetSpawnPedalColor(c);
    }

    private void SetSpawnPedalColor(Color c)
    {
        var clMid = respawnMiddle.colorOverLifetime;
        var clCirc = respawnCircle.colorOverLifetime;
        var maGlow = respawnGlow.main;
        var clDust = respawnDust.colorOverLifetime;

        Gradient g = new Gradient();
        g.mode = GradientMode.Blend;
        GradientAlphaKey[] aKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(0, 0),
            new GradientAlphaKey(1, 0.5f),
            new GradientAlphaKey(0, 1)
        };

        GradientColorKey[] cKeys = new GradientColorKey[]
        {
            new GradientColorKey(c, 0.5f)
        };

        g.SetKeys(cKeys, aKeys);

        clMid.color = new ParticleSystem.MinMaxGradient(g);
        clCirc.color = new ParticleSystem.MinMaxGradient(g);
        maGlow.startColor = c;
        clDust.color = new ParticleSystem.MinMaxGradient(g);
    }

    public void SetName(string newName)
    {
        nameTag.text = newName;
        nameTag.gameObject.SetActive(true);
        nameTag.transform.rotation = Camera.main.transform.rotation;
    }
}
