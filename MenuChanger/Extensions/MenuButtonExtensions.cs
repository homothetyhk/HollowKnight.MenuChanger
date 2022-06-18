using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MenuChanger.Extensions
{
    internal static class MenuButtonExtensions
    {
        public static void ClearEvents(this MenuButton self)
        {
            self.gameObject.GetComponent<EventTrigger>().triggers.Clear();
        }

        public static void AddEvent(this MenuButton self, Action a)
        {
            self.AddEvent(EventTriggerType.Submit, _ => a());
        }

        public static void AddEvent(this MenuButton self, EventTriggerType type, UnityAction<BaseEventData> func)
        {
            EventTrigger.Entry newEvent = new EventTrigger.Entry
            {
                eventID = type
            };

            newEvent.callback.AddListener(func);

            EventTrigger trig = self.gameObject.GetComponent<EventTrigger>();
            if (trig == null)
            {
                trig = self.gameObject.AddComponent<EventTrigger>();
            }

            trig.triggers.Add(newEvent);

            if (type == EventTriggerType.Submit)
            {
                self.AddEvent(EventTriggerType.PointerClick, func);
            }
        }
    }
}