namespace Bhaptics.Tac.Plugin
{
    public interface IBhapticsPlayerPlugin
    {
        object Settings();
        void OnEnable();
        void OnDisable();
    }
}
