using HUD;
using Player;
using UniDi;
using UnityEngine;

public class UniDiInstaller : MonoInstaller
{
    public Camera mainCamera;
    public override void InstallBindings()
    {
        Container.Bind<HudInfo>().AsSingle();
        Container.Bind<ThrustInfo>().AsSingle();
        Container.Bind<PlayerViewInfo>().AsSingle();
        Container.Bind<TargetInfo>().AsSingle();
        Container.Bind<Camera>().FromInstance(mainCamera);
    }
}