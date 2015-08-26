using OpenTK.Input;

namespace BlocksWorld
{
    public interface IGameInputDriver
    {
        KeyboardDevice Keyboard { get; }

        MouseDevice Mouse { get; }
    }
}