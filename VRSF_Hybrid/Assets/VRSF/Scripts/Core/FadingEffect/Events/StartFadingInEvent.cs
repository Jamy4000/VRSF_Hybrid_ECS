namespace VRSF.Core.FadingEffect
{
    public class StartFadingInEvent : EventCallbacks.Event<StartFadingInEvent>
    {
        public StartFadingInEvent() : base("Event called when we want to start a fade in effect")
        {
            FireEvent(this);
        }
    }
}