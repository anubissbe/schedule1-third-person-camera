using MelonLoader;
using ScheduleOneMods.Logging;

namespace ScheduleOneMods.ThirdPersonCamera;

public sealed class Mod : MelonMod
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        Log.SetLogger<Mod>();
        Log.Info("Third Person Camera mod initialized (FINAL - Avatar Camera Hijack)!");
        Log.Info("Instructions:");
        Log.Info("1. Press V to show avatar view (you'll see from front)");
        Log.Info("2. Press F6 to lock it on and control it");
        Log.Info("3. Hold Right Mouse to rotate camera");
        Log.Info("Or: Press F8, then press V to help find the camera");
    }

    public override void OnUpdate()
    {
        AvatarCameraHijack.Update();
    }

    public override void OnLateUpdate()
    {
        AvatarCameraHijack.LateUpdate();
    }
}
