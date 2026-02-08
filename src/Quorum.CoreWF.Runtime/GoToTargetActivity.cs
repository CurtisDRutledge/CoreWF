using System.Activities;
namespace Quorum.CoreWF.Runtime;
public abstract class GoToTargetActivity : NativeActivity
{
    sealed public override bool CanBeScheduledBy(Activity parent) => true;
}