namespace SiegeDefense {
    public interface IInputManager {
        float GetValue(GameInput input);
        bool isPressing(GameInput input);
        bool isTriggered(GameInput input);
        bool isReleased(GameInput input);
        void toggleCursor(bool isVisible);
    }
}
