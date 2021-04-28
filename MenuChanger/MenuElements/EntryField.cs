using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    public abstract class EntryField<T> : IMenuElement
    {
        public MenuPage Parent { get; private set; }
        public GameObject GameObject { get; private set; }
        public InputField InputField { get; private set; }
        public bool Hidden { get; private set; }
        public MenuLabel Label { get; private set; }
        public Vector2 LabelOffset = new Vector2(0f, 55f);

        public EntryField(MenuPage page, string label)
        {
            (GameObject, InputField) = PrefabMenuObjects.BuildEntryField();
            Parent = page;
            page.Add(GameObject);
            GameObject.transform.localScale = new Vector2(0.8f, 0.8f);

            Label = new MenuLabel(page, label, MenuLabel.Style.Body);
            Label.Text.alignment = TextAnchor.UpperCenter;
            MoveTo(Vector2.zero);

            InputField.onValueChanged.AddListener(InvokeChanged);
        }

        public T InputValue
        {
            get => Read(InputField.text);
            set => InputField.text = Write(value);
        }

        public delegate void ChangedHandler(T t);
        protected ChangedHandler ChangedInternal;
        protected void InvokeChanged(string s) => ChangedInternal?.Invoke(Read(s));
        public event ChangedHandler Changed
        {
            add => ChangedInternal += value;
            remove => ChangedInternal -= value;
        }

        public abstract T Read(string input);
        public virtual string Write(T t)
        {
            return t.ToString();
        }

        public void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
            Label.MoveTo(pos + LabelOffset);
        }

        public void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
            Label.Translate(delta);
        }

        public void Show()
        {
            Hidden = false;
            GameObject.SetActive(true);
            Label.Show();
        }

        public void Hide()
        {
            Hidden = true;
            GameObject.SetActive(false);
            Label.Hide();
        }

        public void Destroy()
        {
            GameObject.Destroy(GameObject);
        }

        public static readonly Color INVALID_INPUT_COLOR = Color.red;
        public static readonly Color VALID_INPUT_COLOR = Color.white;
    }


    public class CustomEntryField<T> : EntryField<T>
    {
        public Func<string, T> read;
        public Func<T, string> write;

        public CustomEntryField(MenuPage page, string label, Func<string, T> read, Func<T, string> write) : base(page, label)
        {
            this.read = read;
            this.write = write;
        }

        public override T Read(string input) => read(input);
        public override string Write(T t) => write(t);
    }


    public class TextEntryField : EntryField<string>
    {
        public TextEntryField(MenuPage page, string label) : base(page, label)
        {

        }

        public override string Read(string s)
        {
            return s;
        }
    }

    public class LongEntryField : EntryField<long>
    {
        public LongEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterValidation = InputField.CharacterValidation.Integer;
            InputField.characterLimit = 18;
        }

        public override long Read(string input)
        {
            if (long.TryParse(input, out long val))
            {
                return val;
            }

            return 0;
        }
    }

    public class IntEntryField : EntryField<int>
    {
        public IntEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterValidation = InputField.CharacterValidation.Integer;
            InputField.characterLimit = 10;
        }

        public override int Read(string input)
        {
            if (long.TryParse(input, out long val))
            {
                return (int)Math.Min(Math.Max(val, int.MinValue), int.MaxValue);
            }

            return 0;
        }
    }

    public class ShortEntryField : EntryField<short>
    {
        public ShortEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterValidation = InputField.CharacterValidation.Integer;
            InputField.characterLimit = 5;
        }

        public override short Read(string input)
        {
            if (long.TryParse(input, out long val))
            {
                return (short)Math.Min(Math.Max(val, short.MinValue), short.MaxValue);
            }

            return 0;
        }
    }

    public class ByteEntryField : EntryField<byte>
    {
        public ByteEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterValidation = InputField.CharacterValidation.Integer;
            InputField.characterLimit = 3;
        }

        public override byte Read(string input)
        {
            if (long.TryParse(input, out long val))
            {
                return (byte)Math.Min(Math.Max(val, byte.MinValue), byte.MaxValue);
            }

            return 0;
        }
    }


}
