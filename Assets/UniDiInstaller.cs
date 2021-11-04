using HUD;
using UniDi;

public class UniDiInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<HudInfo>().AsSingle();
    }
}