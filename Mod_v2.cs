using MelonLoader;
using ScheduleOneMods.Logging;

namespace ScheduleOneMods.ThirdPersonCamera;

public sealed class ModV2 : MelonMod
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        Log.SetLogger<ModV2>();
        Log.Info("Third Person Camera mod initialized (V2 - Simplified)!");
        Log.Info("Press F6 to toggle third-person camera");
    }

    public override void OnUpdate()
    {
        CameraControllerV2.Update();
    }

    public override void OnLateUpdate()
    {
        CameraControllerV2.LateUpdate();
    }
}
