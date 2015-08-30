using OpenTK;
using OpenTK.Input;

namespace BlocksWorld
{
    public interface IGameInputDriver
    {
        KeyboardDevice Keyboard { get; }

        MouseDevice Mouse { get; }

        bool GetButtonDown(Key key);
        bool GetButtonUp(Key key);
        bool GetButton(Key key);

        bool GetMouseDown(MouseButton btn);
        bool GetMouseUp(MouseButton btn);
        bool GetMouse(MouseButton btn);

        Vector2 MouseMovement { get; }
		int MouseWheel { get; }
	}
}