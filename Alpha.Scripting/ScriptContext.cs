namespace Alpha.Scripting
{
    public class ScriptContext
    {
        public IScriptableCalendar Calendar;

        public ScriptContext(IScriptableCalendar calendar)
        {
            Calendar = calendar;
        }
    }
}
