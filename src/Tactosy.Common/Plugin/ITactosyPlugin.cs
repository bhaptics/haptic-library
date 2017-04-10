namespace Tactosy.Common.Plugin
{
    public interface ITactosyPlugin
    {
        object Settings();
        void OnEnable();
        void OnDisable();
    }
}
