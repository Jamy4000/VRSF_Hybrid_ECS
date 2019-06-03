namespace VRSF.Core.FadingEffect
{
    public class StartFadingOutEvent : EventCallbacks.Event<StartFadingOutEvent>
    {
        /// <summary>
        /// Whether we wanna fade in directly after the fade out is done.
        /// If this option is at false, you may wanna think about calling the StartFadingInEvent when you need it.
        /// </summary>
        public readonly bool ShouldFadeInWhenDone;

        public StartFadingOutEvent(bool shouldFadeInWhenDone = false) : base("Event called when we want to start a fade out effect")
        {
            ShouldFadeInWhenDone = shouldFadeInWhenDone;
            FireEvent(this);
        }
    }
}