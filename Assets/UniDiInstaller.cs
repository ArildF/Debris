using HUD;
using Player;
using UniDi;

public class UniDiInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<HudInfo>().AsSingle();
        Container.Bind<ThrustInfo>().AsSingle();
        Container.Bind<PlayerViewInfo>().AsSingle();
    }
}