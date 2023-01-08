using System.Reflection;
using UnityEngine.EventSystems;

namespace MenuChanger
{
    public class CustomInputField : InputField
    {
        private static readonly FieldInfo fi = typeof(InputField).GetField("m_ProcessingEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly Event m_ProcessingEvent;
        private static readonly HashSet<KeyCode> keyCodes = new() { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        public CustomInputField()
        {
            m_ProcessingEvent = fi.GetValue(this) as Event;
        }

        public bool AllSelected()
        {
            return caretPositionInternal == text.Length && caretSelectPositionInternal == 0;
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!this.isFocused) return;
            bool allSelected = AllSelected();
            bool flag = false;
            while (Event.PopEvent(this.m_ProcessingEvent))
            {
                if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
                {
                    flag = true;
                    if (allSelected && keyCodes.Contains(this.m_ProcessingEvent.keyCode))
                    {
                        flag = false;
                        this.DeactivateInputField();
                        break;
                        // stop processing events, because we are about to deselect
                    }
                    if (this.KeyPressed(this.m_ProcessingEvent) == InputField.EditState.Finish)
                    {
                        this.DeactivateInputField();
                        break;
                    }
                }
                EventType type = this.m_ProcessingEvent.type;
                if (type - EventType.ValidateCommand <= 1)
                {
                    string commandName = this.m_ProcessingEvent.commandName;
                    if (commandName != null && commandName == "SelectAll")
                    {
                        this.SelectAll();
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                this.UpdateLabel();
            }
            if (!allSelected)
            {
                eventData.Use();
            }
        }
    }

}
