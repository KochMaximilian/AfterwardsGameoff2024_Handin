using System;
using UnityEngine;
using UnityUtils;

public class GameplayItems : PersistentSingleton<GameplayItems>
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnGoToMainMenu += ClearItems;
    }

    private void OnDestroy()
    {
        GameManager.OnGoToMainMenu -= ClearItems;
    }

    public void ClearItems()
    {
        instance = null;
        Destroy(gameObject);
    }
}
