using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseGame : ScriptableObject
{
    public abstract string GetTitle();
    
    public abstract float GameTime { get; }
    
    public virtual void Initialize()
    {  }
    
    public abstract UniTask<int> PlayGame(UIManager uiManager);
}
