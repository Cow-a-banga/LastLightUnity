
//Порядок важен
public enum State
{
    Dialogue,
    Fly,
    Jump,
    Swim,
    Grounded,
    Fall,
}

public interface IState
{
    State Name { get; }
    bool Check();
    void Initialize();
    void Do(float deltaTime);
    void End();
}
